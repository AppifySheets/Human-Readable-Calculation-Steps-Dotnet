namespace HumanReadableCalculationSteps;

/// <summary>
/// Derives, from the operand tree of a value, the caption and the definition steps that
/// <c>FinalCalculationSteps</c> lays out.
/// </summary>
/// <remarks>
/// Walking the tree instead of the recorded step strings makes two things possible
/// (GitHub issue #48):
/// <list type="bullet">
/// <item>a composite that was named with <c>As()</c> is printed as <c>Name[value]</c>
/// wherever the same expression occurs, even through an unnamed copy of it;</item>
/// <item>a composite that is used more than once but was never named receives a generated
/// name (<c>#1</c>, <c>#2</c>, ...) and is derived once, so a calculation that references a
/// shared intermediate at every level grows linearly instead of doubling.</item>
/// </list>
/// Nodes are identified by rendered text, value and precedence, so a recomputed copy of an
/// expression counts as the same node as the original.
/// </remarks>
static class RenderPlan
{
    internal sealed record Result(string Caption, List<string> Steps);

    public static Result Create(ValueWithCaption root)
    {
        // 1. Distinct nodes in post-order: every operand precedes the node that uses it,
        //    which is also the order in which definitions must be printed.
        var order = new List<ValueWithCaption>();
        var seen = new HashSet<NodeKey>();

        void Visit(ValueWithCaption node)
        {
            if (!seen.Add(NodeKey.Of(node))) return;
            foreach (var operand in node.Operands) Visit(operand);
            order.Add(node);
        }

        Visit(root);

        // 2. A node that As() wrapped lends its name to the composite it wraps, so an
        //    unnamed copy of that composite prints by the same name.
        var names = new Dictionary<NodeKey, string>();
        foreach (var node in order.Where(n => n.Precedence == -1 && n.Operands.Count == 1 && n.Operands[0].Precedence > 0))
            names.TryAdd(NodeKey.Of(node.Operands[0]), node._caption);

        // 3. Reference counts and "sits under a composite" flags, walking parents before
        //    operands (reverse post-order). A node rendered once by name, explicit or
        //    generated, contributes a single reference to its operands; an inlined node
        //    contributes as many references as it has itself.
        var references = order.ToDictionary(NodeKey.Of, _ => 0);
        var underComposite = new HashSet<NodeKey>();
        references[NodeKey.Of(root)] = 1;

        foreach (var node in order.AsEnumerable().Reverse())
        {
            var key = NodeKey.Of(node);
            var renderedByName = node.Precedence == -1 || names.ContainsKey(key) || references[key] >= 2;
            var contribution = renderedByName ? 1 : references[key];

            foreach (var operandKey in node.Operands.Select(NodeKey.Of))
            {
                references[operandKey] += contribution;
                if (node.Precedence > 0 || underComposite.Contains(key)) underComposite.Add(operandKey);
            }
        }

        // 4. Generated names for composites referenced more than once without a name.
        var generated = new HashSet<NodeKey>();
        foreach (var key in order.Where(n => n.Precedence > 0).Select(NodeKey.Of))
        {
            if (references[key] < 2 || names.ContainsKey(key)) continue;
            names[key] = $"#{generated.Count + 1}";
            generated.Add(key);
        }

        string? NameOf(ValueWithCaption node) =>
            node.Precedence > 0 && names.TryGetValue(NodeKey.Of(node), out var name) ? name : null;

        // 5. One definition step per named node, in dependency order. Simple "Name = value"
        //    steps of leaves are dropped under a composite, matching how the operators
        //    discard them when combining operands.
        var steps = new List<string>();
        foreach (var node in order)
        {
            var key = NodeKey.Of(node);
            var isUnderComposite = underComposite.Contains(key);

            if (node.Operands.Count == 0)
            {
                // A leaf (As on a literal, From, or the public constructor) carries its own
                // recorded steps; they are the only source of text for it.
                steps.AddRange(node.Steps.Where(step => !isUnderComposite || !ValueWithCaption.IsSimpleAssignmentStep(step)));
            }
            else if (node.Precedence == -1)
            {
                var source = node.Operands[0];
                if (source.Precedence > 0)
                    steps.Add($"{node._caption} = {source.Rebuild(NameOf)} = {node.FormattedValue}");
                else if (source.Precedence == -1)
                    steps.Add($"{node._caption} = {source._caption}[{source.FormattedValue}] = {node.FormattedValue}");
                else if (!isUnderComposite)
                    steps.Add($"{node._caption} = {node.FormattedValue}");
            }
            else if (generated.Contains(key))
            {
                steps.Add($"{names[key]} = {node.Rebuild(NameOf)} = {node.FormattedValue}");
            }
        }

        var caption = root.Precedence > 0 ? root.Rebuild(NameOf) : root._caption;
        return new Result(caption, steps.Distinct().ToList());
    }

    // Two nodes with the same rendered text, value and kind are the same node, whether or
    // not they are the same instance.
    readonly record struct NodeKey(string Caption, decimal Value, int Precedence)
    {
        public static NodeKey Of(ValueWithCaption node) => new(node._caption, node.Value, node.Precedence);
    }
}
