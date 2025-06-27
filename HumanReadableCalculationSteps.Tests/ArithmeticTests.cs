using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    public class ArithmeticTests
    {
        [Fact]
        public void BasicAddition_ShouldCalculateCorrectValue()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a + b;
            
            Assert.Equal(15m, result.Value);
            Assert.Equal("a[10] + b[5]", result.Caption);
        }
        
        [Fact]
        public void BasicSubtraction_ShouldCalculateCorrectValue()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a - b;
            
            Assert.Equal(5m, result.Value);
            Assert.Equal("a[10] - b[5]", result.Caption);
        }
        
        [Fact]
        public void BasicMultiplication_ShouldCalculateCorrectValue()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a * b;
            
            Assert.Equal(50m, result.Value);
            Assert.Equal("a[10] × b[5]", result.Caption);
        }
        
        [Fact]
        public void BasicDivision_ShouldCalculateCorrectValue()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a / b;
            
            Assert.Equal(2m, result.Value);
            Assert.Equal("a[10] ÷ b[5]", result.Caption);
        }
        
        [Fact]
        public void AdditionWithMultiplication_ShouldRespectPrecedence()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            var c = 2m.As("c");
            
            // a + b * c should be a + (b × c) = 10 + 10 = 20
            var result = a + b * c;
            
            Assert.Equal(20m, result.Value);
            Assert.Equal("a[10] + b[5] × c[2]", result.Caption);
        }
        
        [Fact]
        public void MultiplicationWithAddition_ShouldRespectPrecedence()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            var c = 2m.As("c");
            
            // a * b + c should be (a × b) + c = 50 + 2 = 52
            var result = a * b + c;
            
            Assert.Equal(52m, result.Value);
            Assert.Equal("a[10] × b[5] + c[2]", result.Caption);
        }
        
        [Fact]
        public void SubtractionWithMultiplication_ShouldRespectPrecedence()
        {
            var a = 20m.As("a");
            var b = 4m.As("b");
            var c = 2m.As("c");
            
            // a - b * c should be a - (b × c) = 20 - 8 = 12
            var result = a - b * c;
            
            Assert.Equal(12m, result.Value);
            Assert.Equal("a[20] - b[4] × c[2]", result.Caption);
        }
        
        [Fact]
        public void MultiplicationWithSubtraction_ShouldRespectPrecedence()
        {
            var a = 20m.As("a");
            var b = 4m.As("b");
            var c = 2m.As("c");
            
            // a * b - c should be (a × b) - c = 80 - 2 = 78
            var result = a * b - c;
            
            Assert.Equal(78m, result.Value);
            Assert.Equal("a[20] × b[4] - c[2]", result.Caption);
        }
        
        [Fact]
        public void SubtractionWithDivision_ShouldRespectPrecedence()
        {
            var a = 20m.As("a");
            var b = 4m.As("b");
            var c = 2m.As("c");
            
            // a - b / c should be a - (b ÷ c) = 20 - 2 = 18
            var result = a - b / c;
            
            Assert.Equal(18m, result.Value);
            Assert.Equal("a[20] - b[4] ÷ c[2]", result.Caption);
        }
        
        [Fact]
        public void ComplexExpression_AdditionMultiplicationSubtraction_ShouldRespectPrecedence()
        {
            var a = 12m.As("a");
            var b = 3m.As("b");
            var c = 4m.As("c");
            var d = 2m.As("d");
            
            // a + b * c - d should be a + (b × c) - d = 12 + 12 - 2 = 22
            var result = a + b * c - d;
            
            Assert.Equal(22m, result.Value);
            Assert.Equal("a[12] + b[3] × c[4] - d[2]", result.Caption);
        }
        
        [Fact]
        public void ComplexExpression_MultipleMultiplications_ShouldRespectPrecedence()
        {
            var a = 12m.As("a");
            var b = 3m.As("b");
            var c = 4m.As("c");
            var d = 2m.As("d");
            
            // a * b + c * d should be (a × b) + (c × d) = 36 + 8 = 44
            var result = a * b + c * d;
            
            Assert.Equal(44m, result.Value);
            Assert.Equal("a[12] × b[3] + c[4] × d[2]", result.Caption);
        }
        
        [Fact]
        public void MultipleAdditions_ShouldEvaluateLeftToRight()
        {
            var a = 20m.As("a");
            var b = 5m.As("b");
            var c = 2m.As("c");
            
            // a + b + c should be (a + b) + c = 25 + 2 = 27
            var result = a + b + c;
            
            Assert.Equal(27m, result.Value);
            Assert.Equal("a[20] + b[5] + c[2]", result.Caption);
        }
        
        [Fact]
        public void MultipleSubtractions_ShouldEvaluateLeftToRight()
        {
            var a = 20m.As("a");
            var b = 5m.As("b");
            var c = 2m.As("c");
            
            // a - b - c should be (a - b) - c = 15 - 2 = 13
            var result = a - b - c;
            
            Assert.Equal(13m, result.Value);
            Assert.Equal("a[20] - b[5] - c[2]", result.Caption);
        }
        
        [Fact]
        public void MultipleMultiplications_ShouldEvaluateLeftToRight()
        {
            var a = 20m.As("a");
            var b = 5m.As("b");
            var c = 2m.As("c");
            
            // a * b * c should be (a × b) × c = 100 × 2 = 200
            var result = a * b * c;
            
            Assert.Equal(200m, result.Value);
            Assert.Equal("a[20] × b[5] × c[2]", result.Caption);
        }
        
        [Fact]
        public void MultipleDivisions_ShouldEvaluateLeftToRight()
        {
            var a = 20m.As("a");
            var b = 5m.As("b");
            var c = 2m.As("c");
            
            // a / b / c should be (a ÷ b) ÷ c = 4 ÷ 2 = 2
            var result = a / b / c;
            
            Assert.Equal(2m, result.Value);
            Assert.Equal("a[20] ÷ b[5] ÷ c[2]", result.Caption);
        }
        
        [Fact]
        public void NestedPrecedence_MultipleHighPrecedenceOperations()
        {
            var a = 8m.As("a");
            var b = 2m.As("b");
            var c = 4m.As("c");
            var d = 3m.As("d");
            var e = 5m.As("e");
            
            // a + b * c + d * e should be a + (b × c) + (d × e) = 8 + 8 + 15 = 31
            var result = a + b * c + d * e;
            
            Assert.Equal(31m, result.Value);
            Assert.Equal("a[8] + b[2] × c[4] + d[3] × e[5]", result.Caption);
        }
        
        [Fact]
        public void NestedPrecedence_MixedOperations()
        {
            var a = 8m.As("a");
            var b = 2m.As("b");
            var c = 4m.As("c");
            var d = 3m.As("d");
            var e = 5m.As("e");
            
            // a * b + c * d - e should be (a × b) + (c × d) - e = 16 + 12 - 5 = 23
            var result = a * b + c * d - e;
            
            Assert.Equal(23m, result.Value);
            Assert.Equal("a[8] × b[2] + c[4] × d[3] - e[5]", result.Caption);
        }
        
        [Fact]
        public void NestedPrecedence_DivisionWithAdditionSubtraction()
        {
            var a = 8m.As("a");
            var b = 2m.As("b");
            var c = 4m.As("c");
            var d = 3m.As("d");
            var e = 5m.As("e");
            
            // a / b - c + d * e should be (a ÷ b) - c + (d × e) = 4 - 4 + 15 = 15
            var result = a / b - c + d * e;
            
            Assert.Equal(15m, result.Value);
            Assert.Equal("a[8] ÷ b[2] - c[4] + d[3] × e[5]", result.Caption);
        }
        
        [Fact]
        public void OriginalExample_TaxCalculation()
        {
            var basePrice = 100m.As("საბაზო ფასი");
            var taxRate = 0.18m.As("დღგ");
            
            var tax = basePrice * taxRate;
            
            Assert.Equal(18m, tax.Value);
            Assert.Equal("საბაზო ფასი[100] × დღგ[0.18]", tax.Caption);
        }
        
        [Fact]
        public void OriginalExample_DiscountedPrice()
        {
            var basePrice = 100m.As("საბაზო ფასი");
            var discount = 15m.As("ფასდაკლება");
            
            var discountedPrice = basePrice - discount;
            
            Assert.Equal(85m, discountedPrice.Value);
            Assert.Equal("საბაზო ფასი[100] - ფასდაკლება[15]", discountedPrice.Caption);
        }
        
        [Fact]
        public void OriginalExample_FinalPriceWithPrecedence()
        {
            var basePrice = 100m.As("საბაზო ფასი");
            var taxRate = 0.18m.As("დღგ");
            var discount = 15m.As("ფასდაკლება");
            var multiplier = 120.0m.As("ასოცი");

            var tax = basePrice * taxRate;
            var discountedPrice = basePrice - discount;
            
            // discountedPrice + tax * multiplier should be discountedPrice + (tax × multiplier)
            // = 85 + (18 × 120) = 85 + 2160 = 2245
            var finalPrice = discountedPrice + tax * multiplier;
            
            Assert.Equal(2245m, finalPrice.Value);
            Assert.Equal("საბაზო ფასი[100] - ფასდაკლება[15] + საბაზო ფასი[100] × დღგ[0.18] × ასოცი[120]", finalPrice.Caption);
        }
        
        [Theory]
        [InlineData(10, 5, 2, 20)] // 10 + 5 * 2 = 10 + 10 = 20
        [InlineData(20, 3, 4, 32)] // 20 + 3 * 4 = 20 + 12 = 32
        [InlineData(15, 2, 3, 21)] // 15 + 2 * 3 = 15 + 6 = 21
        public void AdditionWithMultiplication_Theory(decimal a, decimal b, decimal c, decimal expected)
        {
            var varA = a.As("a");
            var varB = b.As("b");
            var varC = c.As("c");
            
            var result = varA + varB * varC;
            
            Assert.Equal(expected, result.Value);
        }
        
        [Theory]
        [InlineData(20, 4, 2, 12)] // 20 - 4 * 2 = 20 - 8 = 12
        [InlineData(30, 5, 3, 15)] // 30 - 5 * 3 = 30 - 15 = 15
        [InlineData(25, 3, 4, 13)] // 25 - 3 * 4 = 25 - 12 = 13
        public void SubtractionWithMultiplication_Theory(decimal a, decimal b, decimal c, decimal expected)
        {
            var varA = a.As("a");
            var varB = b.As("b");
            var varC = c.As("c");
            
            var result = varA - varB * varC;
            
            Assert.Equal(expected, result.Value);
        }
        
        [Fact]
        public void ParenthesesOverridePrecedence_AdditionBeforeMultiplication()
        {
            var a = 2m.As("a");
            var b = 3m.As("b");
            var c = 4m.As("c");
            
            // (a + b) * c should be (2 + 3) * 4 = 5 * 4 = 20
            var result = (a + b) * c;
            
            Assert.Equal(20m, result.Value);
            Assert.Equal("(a[2] + b[3]) × c[4]", result.Caption);
        }
        
        [Fact]
        public void ParenthesesOverridePrecedence_SubtractionBeforeMultiplication()
        {
            var a = 10m.As("a");
            var b = 3m.As("b");
            var c = 2m.As("c");
            
            // (a - b) * c should be (10 - 3) * 2 = 7 * 2 = 14
            var result = (a - b) * c;
            
            Assert.Equal(14m, result.Value);
            Assert.Equal("(a[10] - b[3]) × c[2]", result.Caption);
        }
        
        [Fact]
        public void ParenthesesOverridePrecedence_AdditionBeforeDivision()
        {
            var a = 12m.As("a");
            var b = 3m.As("b");
            var c = 5m.As("c");
            
            // (a + b) / c should be (12 + 3) / 5 = 15 / 5 = 3
            var result = (a + b) / c;
            
            Assert.Equal(3m, result.Value);
            Assert.Equal("(a[12] + b[3]) ÷ c[5]", result.Caption);
        }
        
        [Fact]
        public void ParenthesesOverridePrecedence_MultiplicationBeforeAddition()
        {
            var a = 2m.As("a");
            var b = 3m.As("b");
            var c = 4m.As("c");
            
            // a * (b + c) should be 2 * (3 + 4) = 2 * 7 = 14
            var result = a * (b + c);
            
            Assert.Equal(14m, result.Value);
            Assert.Equal("a[2] × (b[3] + c[4])", result.Caption);
        }
        
        [Fact]
        public void ParenthesesOverridePrecedence_DivisionBeforeSubtraction()
        {
            var a = 20m.As("a");
            var b = 8m.As("b");
            var c = 2m.As("c");
            
            // a / (b - c) should be 20 / (8 - 2) = 20 / 6 = 3.333...
            var result = a / (b - c);
            
            Assert.Equal(20m / 6m, result.Value);
            Assert.Equal("a[20] ÷ (b[8] - c[2])", result.Caption);
        }
        
        [Fact]
        public void ComplexParentheses_NestedExpressions()
        {
            var a = 2m.As("a");
            var b = 3m.As("b");
            var c = 4m.As("c");
            var d = 5m.As("d");
            
            // (a + b) * (c - d) should be (2 + 3) * (4 - 5) = 5 * (-1) = -5
            var result = (a + b) * (c - d);
            
            Assert.Equal(-5m, result.Value);
            Assert.Equal("(a[2] + b[3]) × (c[4] - d[5])", result.Caption);
        }
        
        [Fact]
        public void ComplexParentheses_DivisionOfSums()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            var c = 3m.As("c");
            var d = 2m.As("d");
            
            // (a + b) / (c + d) should be (10 + 5) / (3 + 2) = 15 / 5 = 3
            var result = (a + b) / (c + d);
            
            Assert.Equal(3m, result.Value);
            Assert.Equal("(a[10] + b[5]) ÷ (c[3] + d[2])", result.Caption);
        }
        
        [Fact]
        public void ComplexParentheses_MixedOperationsWithOverride()
        {
            var a = 6m.As("a");
            var b = 2m.As("b");
            var c = 3m.As("c");
            var d = 4m.As("d");
            
            // a * (b + c) - d should be 6 * (2 + 3) - 4 = 6 * 5 - 4 = 30 - 4 = 26
            var result = a * (b + c) - d;
            
            Assert.Equal(26m, result.Value);
            Assert.Equal("a[6] × (b[2] + c[3]) - d[4]", result.Caption);
        }
        
        [Fact]
        public void ComplexParentheses_AdditionOfProducts()
        {
            var a = 2m.As("a");
            var b = 3m.As("b");
            var c = 4m.As("c");
            var d = 5m.As("d");
            var e = 1m.As("e");
            
            // (a * b) + (c * d) - e should be (2 * 3) + (4 * 5) - 1 = 6 + 20 - 1 = 25
            var result = (a * b) + (c * d) - e;
            
            Assert.Equal(25m, result.Value);
            Assert.Equal("a[2] × b[3] + c[4] × d[5] - e[1]", result.Caption);
        }
        
        [Fact]
        public void WrappingOperationResult_WithNewCaption()
        {
            var basePrice = 100m.As("საბაზო ფასი");
            var taxRate = 0.18m.As("დღგ");
            
            // Calculate tax and wrap it with a new caption
            var tax = (basePrice * taxRate).As("TaxValue");
            
            Assert.Equal(18m, tax.Value);
            Assert.Equal("TaxValue", tax.Caption);
            
            Assert.Equal("TaxValue =\n  საბაზო ფასი[100] \n× დღგ[0.18] \n= 18", tax.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappingOperationResult_UsedInFurtherCalculations()
        {
            var basePrice = 100m.As("საბაზო ფასი");
            var taxRate = 0.18m.As("დღგ");
            var discount = 15m.As("ფასდაკლება");
            
            var tax = (basePrice * taxRate).As("TaxValue");
            var discountedPrice = basePrice - discount;
            
            // Use the wrapped tax value in further calculations
            var finalPrice = discountedPrice + tax;
            
            Assert.Equal(103m, finalPrice.Value); // 85 + 18 = 103
            Assert.Equal("საბაზო ფასი[100] - ფასდაკლება[15] + TaxValue", finalPrice.Caption);
            
            Assert.Equal("TaxValue =\n  საბაზო ფასი[100] \n× დღგ[0.18] \n= 18", tax.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappingComplexExpression_WithSimpleName()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            var c = 2m.As("c");
            var d = 3m.As("d");
            
            // Wrap a complex expression with a simple name
            var complexCalc = (a + b * c - d).As("Result");
            var multiplier = 4m.As("multiplier");
            
            var finalResult = complexCalc * multiplier;
            
            Assert.Equal(68m, finalResult.Value); // (10 + 10 - 3) * 4 = 17 * 4 = 68
            Assert.Equal("Result × multiplier[4]", finalResult.Caption);
            
            // Check that the Result definition is captured
            Assert.Single(complexCalc.CalculationSteps);
            Assert.Equal("Result = a[10] + b[5] × c[2] - d[3] = 17", complexCalc.CalculationSteps[0]);
            
            // Check that finalResult shows the Result definition
            Assert.Equal(1, finalResult.CalculationSteps.Count);
            Assert.Equal("Result = a[10] + b[5] × c[2] - d[3] = 17", finalResult.CalculationSteps[0]);
            
            Assert.Equal("Result =\n  a[10] \n+ b[5]  \n× c[2]  \n- d[3] \n= 17", complexCalc.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappingNestedParentheses_WithMeaningfulName()
        {
            var length = 12m.As("length");
            var width = 8m.As("width");
            var height = 5m.As("height");
            
            // Calculate area and volume with meaningful names
            var area = (length * width).As("Area");
            var volume = (area * height).As("Volume");
            
            Assert.Equal(96m, area.Value);
            Assert.Equal("Area", area.Caption);
            
            Assert.Equal(480m, volume.Value);
            Assert.Equal("Volume", volume.Caption);
            
            Assert.Equal("Area = \n  length[12] \n× width[8] \n= 96", area.FinalCalculationSteps);
            Assert.Equal(
"""
Area = 
  length[12] 
× width[8] 
= 96

Volume =
  Area[96] 
× height[5] 
= 480
""", volume.FinalCalculationSteps);
        }

        [Fact]
        public void MultipleWrappedOperations_InSingleExpression()
        {
            var price1 = 50m.As("price1");
            var price2 = 30m.As("price2");
            var tax1Rate = 0.1m.As("tax1Rate");
            var tax2Rate = 0.15m.As("tax2Rate");

            var tax1 = (price1 * tax1Rate).As("Tax1");
            var tax2 = (price2 * tax2Rate).As("Tax2");

            var totalTax = (tax1 + tax2).As("TotalTax");

            Assert.Equal(5m, tax1.Value);
            Assert.Equal(4.5m, tax2.Value);
            Assert.Equal(9.5m, totalTax.Value);
            Assert.Equal("TotalTax", totalTax.Caption);

            // Check calculation steps
            Assert.Equal(3, totalTax.CalculationSteps.Count);
            Assert.Equal("Tax1 = price1[50] × tax1Rate[0.1] = 5", totalTax.CalculationSteps[0]);
            Assert.Equal("Tax2 = price2[30] × tax2Rate[0.15] = 4.5", totalTax.CalculationSteps[1]);
            Assert.Equal("TotalTax = Tax1[5] + Tax2[4.5] = 9.5", totalTax.CalculationSteps[2]);

            Assert.Equal(
"""
Tax1 = 
  price1[50] 
× tax1Rate[0.1] 
= 5

Tax2 = 
  price2[30] 
× tax2Rate[0.15] 
= 4.5

TotalTax =
  Tax1[5] 
+ Tax2[4.5] 
= 9.5
""", totalTax.FinalCalculationSteps);

        }
        
        [Fact]
        public void WrappedValue_MaintainsPrecedenceCorrectly()
        {
            var a = 5m.As("a");
            var b = 2m.As("b");
            var c = 3m.As("c");
            
            var product = (a * b).As("Product");
            var result = product + c;
            
            Assert.Equal(13m, result.Value); // (5 * 2) + 3 = 10 + 3 = 13
            Assert.Equal("Product + c[3]", result.Caption);
            
            Assert.Equal("Product =\n  a[5] \n× b[2] \n= 10", product.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappedValue_InParenthesesExpression()
        {
            var x = 6m.As("x");
            var y = 4m.As("y");
            var z = 2m.As("z");
            
            var difference = (x - y).As("Diff");
            var result = difference * z;
            
            Assert.Equal(4m, result.Value); // (6 - 4) * 2 = 2 * 2 = 4
            Assert.Equal("Diff × z[2]", result.Caption);
            
            Assert.Equal("Diff = \n  x[6] \n- y[4] \n= 2", difference.FinalCalculationSteps);
        }
        
        [Fact]
        public void MultipleWrappingLevels()
        {
            var length = 10m.As("length");
            var width = 5m.As("width");
            var height = 3m.As("height");
            
            var area = (length * width).As("Area");
            var volume = (area * height).As("Volume");
            var density = 2.5m.As("density");
            var mass = (volume * density).As("Mass");
            
            Assert.Equal(50m, area.Value);
            Assert.Equal("Area", area.Caption);
            
            Assert.Equal(150m, volume.Value);
            Assert.Equal("Volume", volume.Caption);
            
            Assert.Equal(375m, mass.Value);
            Assert.Equal("Mass", mass.Caption);
            
            Assert.Equal("Area = \n  length[10] \n× width[5] \n= 50", area.FinalCalculationSteps);
            Assert.Equal(
"""
Area = 
  length[10] 
× width[5] 
= 50

Volume =
  Area[50] 
× height[3] 
= 150
""", volume.FinalCalculationSteps);
            Assert.Equal(
"""
Area = 
  length[10] 
× width[5] 
= 50

Volume =
  Area[50] 
× height[3] 
= 150

Mass = 
  Volume[150] 
× density[2.5] 
= 375
""", mass.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappedValue_UsedInComplexExpression()
        {
            var principal = 1000m.As("principal");
            var rate = 0.05m.As("rate");
            var time = 2m.As("time");
            
            var interest = (principal * rate * time).As("Interest");
            var fee = 25m.As("fee");
            var total = principal + interest - fee;
            
            Assert.Equal(100m, interest.Value); // 1000 * 0.05 * 2 = 100
            Assert.Equal(1075m, total.Value); // 1000 + 100 - 25 = 1075
            Assert.Equal("principal[1,000] + Interest - fee[25]", total.Caption);
            
            Assert.Equal("Interest =\n  principal[1,000] \n× rate[0.05]  \n× time[2] \n= 100", interest.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappingPreservesOriginalValue()
        {
            var a = 7m.As("a");
            var b = 3m.As("b");
            
            var originalResult = a + b;
            var wrappedResult = (a + b).As("Sum");
            
            Assert.Equal(originalResult.Value, wrappedResult.Value);
            Assert.NotEqual(originalResult.Caption, wrappedResult.Caption);
            Assert.Equal("Sum", wrappedResult.Caption);
            
            Assert.Equal("Sum = \n  a[7] \n+ b[3] \n= 10", wrappedResult.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappedValue_WithDivisionPrecedence()
        {
            var total = 120m.As("total");
            var count = 8m.As("count");
            var bonus = 5m.As("bonus");
            
            var average = (total / count).As("Average");
            var finalAmount = average + bonus;
            
            Assert.Equal(15m, average.Value); // 120 / 8 = 15
            Assert.Equal(20m, finalAmount.Value); // 15 + 5 = 20
            Assert.Equal("Average + bonus[5]", finalAmount.Caption);
            
            Assert.Equal("Average =\n  total[120] \n÷ count[8] \n= 15", average.FinalCalculationSteps);
        }
        
        [Fact]
        public void CalculationSteps_SimpleOperationsHaveNoSteps()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a + b;
            
            Assert.Empty(result.CalculationSteps);
        }
        
        [Fact]
        public void CalculationSteps_WrappedValueUsedInCalculation()
        {
            var basePrice = 100m.As("basePrice");
            var taxRate = 0.18m.As("taxRate");
            var discount = 15m.As("discount");
            
            var tax = (basePrice * taxRate).As("Tax");
            var discountedPrice = basePrice - discount;
            var finalPrice = discountedPrice + tax;
            
            // Tax should have definition step (it's a wrapped computed expression)
            Assert.Single(tax.CalculationSteps);
            Assert.Equal("Tax = basePrice[100] × taxRate[0.18] = 18", tax.CalculationSteps[0]);
            
            // discountedPrice should have no steps (simple operation)
            Assert.Empty(discountedPrice.CalculationSteps);
            
            // finalPrice should have calculation steps including Tax definition
            Assert.Equal(1, finalPrice.CalculationSteps.Count);
            Assert.Equal("Tax = basePrice[100] × taxRate[0.18] = 18", finalPrice.CalculationSteps[0]);
            // No intermediate calculation steps are generated anymore
            
            Assert.Equal("Tax = \n  basePrice[100] \n× taxRate[0.18] \n= 18", tax.FinalCalculationSteps);
        }
        
        [Fact]
        public void CalculationSteps_ComplexNestedCalculations()
        {
            var length = 10m.As("length");
            var width = 5m.As("width");
            var height = 3m.As("height");
            var density = 2.5m.As("density");
            
            var area = (length * width).As("Area");
            var volume = (area * height).As("Volume");
            var mass = volume * density;
            
            // Area should have definition step
            Assert.Single(area.CalculationSteps);
            Assert.Equal("Area = length[10] × width[5] = 50", area.CalculationSteps[0]);
            
            // Volume should have steps showing Area definition and Volume definition
            Assert.Equal(2, volume.CalculationSteps.Count);
            Assert.Equal("Area = length[10] × width[5] = 50", volume.CalculationSteps[0]);
            Assert.Equal("Volume = Area[50] × height[3] = 150", volume.CalculationSteps[1]);
            
            // Mass should show all calculation steps
            Assert.Equal(2, mass.CalculationSteps.Count);
            Assert.Equal("Area = length[10] × width[5] = 50", mass.CalculationSteps[0]);
            Assert.Equal("Volume = Area[50] × height[3] = 150", mass.CalculationSteps[1]);
            
            Assert.Equal("Area = \n  length[10] \n× width[5] \n= 50", area.FinalCalculationSteps);
            Assert.Equal(
"""
Area = 
  length[10] 
× width[5] 
= 50

Volume =
  Area[50] 
× height[3] 
= 150
""", volume.FinalCalculationSteps);
        }
        
        [Fact]
        public void CalculationSteps_MultipleWrappedValues()
        {
            var price1 = 100m.As("price1");
            var price2 = 50m.As("price2");
            var taxRate1 = 0.1m.As("taxRate1");
            var taxRate2 = 0.15m.As("taxRate2");
            
            var tax1 = (price1 * taxRate1).As("Tax1");
            var tax2 = (price2 * taxRate2).As("Tax2");
            var totalTax = (tax1 + tax2).As("TotalTax");
            
            Assert.Equal(3, totalTax.CalculationSteps.Count);
            Assert.Equal("Tax1 = price1[100] × taxRate1[0.1] = 10", totalTax.CalculationSteps[0]);
            Assert.Equal("Tax2 = price2[50] × taxRate2[0.15] = 7.5", totalTax.CalculationSteps[1]);
            Assert.Equal("TotalTax = Tax1[10] + Tax2[7.5] = 17.5", totalTax.CalculationSteps[2]);
            
            Assert.Equal(
"""
Tax1 = 
  price1[100] 
× taxRate1[0.1] 
= 10

Tax2 = 
  price2[50] 
× taxRate2[0.15] 
= 7.5

TotalTax =
  Tax1[10] 
+ Tax2[7.5] 
= 17.5
""", totalTax.FinalCalculationSteps);
        }
        
        [Fact]
        public void CalculationSteps_DeepNesting()
        {
            var a = 5m.As("a");
            var b = 3m.As("b");
            var c = 2m.As("c");
            var d = 4m.As("d");
            
            var sum1 = (a + b).As("Sum1");
            var sum2 = (c + d).As("Sum2");
            var product1 = sum1 * sum2;
            var finalResult = product1 + a;
            
            // product1 should have steps showing Sum1 def and Sum2 def
            Assert.Equal(2, product1.CalculationSteps.Count);
            Assert.Equal("Sum1 = a[5] + b[3] = 8", product1.CalculationSteps[0]);
            Assert.Equal("Sum2 = c[2] + d[4] = 6", product1.CalculationSteps[1]);
            // No intermediate calculation steps are generated anymore
            
            // finalResult should have all previous steps
            Assert.Equal(2, finalResult.CalculationSteps.Count);
            Assert.Equal("Sum1 = a[5] + b[3] = 8", finalResult.CalculationSteps[0]);
            Assert.Equal("Sum2 = c[2] + d[4] = 6", finalResult.CalculationSteps[1]);
            
            Assert.Equal("Sum1 = \n  a[5] \n+ b[3] \n= 8", sum1.FinalCalculationSteps);
            Assert.Equal("Sum2 = \n  c[2] \n+ d[4] \n= 6", sum2.FinalCalculationSteps);
        }
        
        [Fact]
        public void SimpleVariableAssignment_ShouldShowOnSeparateLines()
        {
            var basePrice = 100m.As("BasePrice");
            var taxRate = 0.18m.As("TaxRate");
            
            // Simple variable assignments should appear in FinalCalculationSteps
            Assert.Equal("BasePrice = 100", basePrice.FinalCalculationSteps);
            Assert.Equal("TaxRate = 0.18", taxRate.FinalCalculationSteps);
        }
        
        [Fact]
        public void SimpleVariableAssignment_UsedInCalculation_ShouldShowInOperands()
        {
            var basePrice = 100m.As("BasePrice");
            var taxRate = 0.18m.As("TaxRate");
            
            var tax = basePrice * taxRate;
            
            // Simple variables should appear with their values in the calculation
            Assert.Equal("BasePrice[100] × TaxRate[0.18]", tax.Caption);
            Assert.Equal(18m, tax.Value);
        }
        
        [Fact]
        public void MixedSimpleAndCalculatedAssignments_ShouldShowOnlyCalculationSteps()
        {
            var basePrice = 100m.As("BasePrice");
            var taxRate = 0.18m.As("TaxRate");
            var discount = 15m.As("Discount");
            
            var tax = (basePrice * taxRate).As("Tax");
            var discountedPrice = basePrice - discount;
            var finalPrice = (discountedPrice + tax).As("FinalPrice");
            
            // finalPrice should show only calculation steps, not simple assignments
            var expectedSteps =
"""
Tax = 
  BasePrice[100] 
× TaxRate[0.18] 
= 18

FinalPrice =
  BasePrice[100] 
- Discount[15]  
+ Tax[18] 
= 103
""";
            
            Assert.Equal(expectedSteps, finalPrice.FinalCalculationSteps);
        }
        
        [Fact]
        public void ComplexCalculation_WithMultipleSimpleVariables_ShouldShowCorrectSteps()
        {
            var length = 10m.As("Length");
            var width = 5m.As("Width");
            var height = 3m.As("Height");
            var density = 2.5m.As("Density");
            
            var area = (length * width).As("Area");
            var volume = (area * height).As("Volume");
            var mass = (volume * density).As("Mass");
            
            // mass should show all calculation steps but not simple variable definitions
            var expectedSteps = 
"""
Area = 
  Length[10] 
× Width[5] 
= 50

Volume =
  Area[50] 
× Height[3] 
= 150

Mass = 
  Volume[150] 
× Density[2.5] 
= 375
""";
            
            Assert.Equal(expectedSteps, mass.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappedSimpleVariable_ShouldShowDefinition()
        {
            var price = 100m.As("Price");
            
            // Individual simple variables should show their definition
            Assert.Equal("Price = 100", price.FinalCalculationSteps);
        }
        
        [Fact]
        public void WrappedCalculatedValue_ShouldShowCalculationWithSimpleVariableValues()
        {
            var price = 100m.As("Price");
            var taxRate = 0.18m.As("TaxRate");
            
            var tax = (price * taxRate).As("Tax");
            
            // Wrapped calculated values should show the calculation with variable values
            Assert.Equal("Tax = \n  Price[100] \n× TaxRate[0.18] \n= 18", tax.FinalCalculationSteps);
        }
    }
}