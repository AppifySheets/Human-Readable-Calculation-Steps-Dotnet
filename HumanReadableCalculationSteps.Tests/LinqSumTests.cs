using System.Linq;
using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    public class LinqSumTests
    {
        [Fact]
        public void Sum_WithLambdaExpression_ThreeItems_ShouldCreateExpandedCaption()
        {
            // Arrange
            var items = new[]
            {
                new { Salary = 100m.As("Salary1") },
                new { Salary = 200m.As("Salary2") },
                new { Salary = 150m.As("Salary3") }
            };

            // Act
            var total = items.Sum(x => x.Salary);

            // Assert
            Assert.Equal(450m, total.Value);
            Assert.Equal("Salary1[100] + Salary2[200] + Salary3[150]", total.Caption);
        }

        [Fact]
        public void Sum_WithDirectCollection_ThreeItems_ShouldCreateExpandedCaption()
        {
            // Arrange
            var values = new[]
            {
                10m.As("A"),
                20m.As("B"),
                30m.As("C")
            };

            // Act
            var total = values.Sum();

            // Assert
            Assert.Equal(60m, total.Value);
            Assert.Equal("A[10] + B[20] + C[30]", total.Caption);
        }

        [Fact]
        public void Sum_WithComplexValueWithCaptionObjects_TwoItems_ShouldPreserveCalculationSteps()
        {
            // Arrange
            var base1 = 50m.As("Base1");
            var rate1 = 1.1m.As("Rate1");
            var advance1 = (base1 * rate1).As("Advance1");

            var base2 = 75m.As("Base2");
            var rate2 = 1.2m.As("Rate2");
            var advance2 = (base2 * rate2).As("Advance2");

            var items = new[]
            {
                new { Advance = advance1 },
                new { Advance = advance2 }
            };

            // Act
            var totalAdvance = items.Sum(i => i.Advance);

            // Assert
            Assert.Equal(145m, totalAdvance.Value); // (50 * 1.1) + (75 * 1.2) = 55 + 90 = 145
            Assert.Equal("Advance1[55] + Advance2[90]", totalAdvance.Caption);
            
            // Should preserve calculation steps from both advance calculations
            Assert.Contains("Advance1 = Base1[50] × Rate1[1.1] = 55", totalAdvance.CalculationSteps);
            Assert.Contains("Advance2 = Base2[75] × Rate2[1.2] = 90", totalAdvance.CalculationSteps);
        }

        [Fact]
        public void Sum_WithEmptyCollection_ShouldReturnZeroWithEmptyCaption()
        {
            // Arrange
            var emptyItems = new object[0];

            // Act
            var total = emptyItems.Sum(x => 10m.As("Test"));

            // Assert
            Assert.Equal(0m, total.Value);
            Assert.Equal("0", total.Caption);
        }

        [Fact]
        public void Sum_WithSingleItem_ShouldReturnItemCaption()
        {
            // Arrange
            var items = new[]
            {
                new { Value = 100m.As("SingleValue") }
            };

            // Act
            var total = items.Sum(x => x.Value);

            // Assert
            Assert.Equal(100m, total.Value);
            Assert.Equal("SingleValue[100]", total.Caption);
        }

        [Fact]
        public void Sum_WithMixedSimpleAndWrappedValues_TwoItems_ShouldCombineCorrectly()
        {
            // Arrange
            var simpleValue = 50m.As("Simple");
            var complexValue = (100m.As("Base") * 1.5m.As("Multiplier")).As("Complex");
            
            var values = new[] { simpleValue, complexValue };

            // Act
            var total = values.Sum();

            // Assert
            Assert.Equal(200m, total.Value); // 50 + (100 * 1.5) = 50 + 150 = 200
            Assert.Equal("Simple[50] + Complex[150]", total.Caption);
            
            // Should preserve calculation steps from complex value
            Assert.Contains("Complex = Base[100] × Multiplier[1.5] = 150", total.CalculationSteps);
        }

        [Fact]
        public void Sum_RealWorldScenario_AdvanceInSalary_ThreeItems()
        {
            // Arrange - Simulate real-world scenario
            var employees = new[]
            {
                new { V1 = new { AdvanceInSalary = 500m.As("Employee1Advance") } },
                new { V1 = new { AdvanceInSalary = 750m.As("Employee2Advance") } },
                new { V1 = new { AdvanceInSalary = 300m.As("Employee3Advance") } }
            };

            // Act - This is the exact scenario the user wants to support
            var totalAdvance = employees.Sum(r => r.V1.AdvanceInSalary);

            // Assert
            Assert.Equal(1550m, totalAdvance.Value);
            Assert.Equal("Employee1Advance[500] + Employee2Advance[750] + Employee3Advance[300]", totalAdvance.Caption);
        }

        [Fact]
        public void Sum_WithCalculatedAdvances_TwoItems_ShouldShowCompleteCalculationSteps()
        {
            // Arrange - More complex real-world scenario with calculated advances
            var employees = new[]
            {
                new 
                { 
                    V1 = new 
                    { 
                        AdvanceInSalary = (2000m.As("BaseSalary1") * 0.25m.As("AdvanceRate")).As("Employee1Advance")
                    } 
                },
                new 
                { 
                    V1 = new 
                    { 
                        AdvanceInSalary = (3000m.As("BaseSalary2") * 0.3m.As("AdvanceRate")).As("Employee2Advance")
                    } 
                }
            };

            // Act
            var totalAdvance = employees.Sum(r => r.V1.AdvanceInSalary);

            // Assert
            Assert.Equal(1400m, totalAdvance.Value); // (2000 * 0.25) + (3000 * 0.3) = 500 + 900 = 1400
            Assert.Equal("Employee1Advance[500] + Employee2Advance[900]", totalAdvance.Caption);
            
            // Should show calculation steps for both advances
            Assert.Contains("Employee1Advance = BaseSalary1[2,000] × AdvanceRate[0.25] = 500", totalAdvance.CalculationSteps);
            Assert.Contains("Employee2Advance = BaseSalary2[3,000] × AdvanceRate[0.3] = 900", totalAdvance.CalculationSteps);
        }

        [Fact]
        public void Sum_WithNestedCalculations_ShouldDisplayFinalCalculationSteps()
        {
            // Arrange
            var advance1 = (2000m.As("Salary1") * 0.25m.As("Rate")).As("Advance1");
            var advance2 = (3000m.As("Salary2") * 0.3m.As("Rate")).As("Advance2");
            
            var values = new[] { advance1, advance2 };
            var totalAdvance = values.Sum().As("TotalAdvances");

            // Act & Assert - Check FinalCalculationSteps
            var expectedSteps = """
                Advance1 = Salary1[2,000] × Rate[0.25] = 500

                Advance2 = Salary2[3,000] × Rate[0.3] = 900

                TotalAdvances = Advance1[500] + Advance2[900] = 1,400
                """;

            Assert.Equal(expectedSteps, totalAdvance.FinalCalculationSteps);
        }

        [Fact]
        public void Sum_WithMoreThanThreeItems_ShouldUseCompactFormat()
        {
            // Arrange - More than 3 items should use compact format
            var employees = new[]
            {
                new { V1 = new { AdvanceInSalary = 500m.As("Employee1Advance") } },
                new { V1 = new { AdvanceInSalary = 750m.As("Employee2Advance") } },
                new { V1 = new { AdvanceInSalary = 300m.As("Employee3Advance") } },
                new { V1 = new { AdvanceInSalary = 400m.As("Employee4Advance") } },
                new { V1 = new { AdvanceInSalary = 600m.As("Employee5Advance") } }
            };

            // Act
            var totalAdvance = employees.Sum(r => r.V1.AdvanceInSalary);

            // Assert
            Assert.Equal(2550m, totalAdvance.Value); // 500 + 750 + 300 + 400 + 600 = 2550
            Assert.Equal("Sum(Advance, count(5))[2,550]", totalAdvance.Caption);
        }

        [Fact]
        public void Sum_WithFourItems_ShouldUseCompactFormat()
        {
            // Arrange - Exactly 4 items (more than 3) should use compact format
            var values = new[]
            {
                100m.As("Value1"),
                200m.As("Value2"),
                150m.As("Value3"),
                250m.As("Value4")
            };

            // Act
            var total = values.Sum();

            // Assert
            Assert.Equal(700m, total.Value);
            Assert.Equal("Sum(Value, count(4))[700]", total.Caption);
        }

        [Fact]
        public void Sum_WithManyCalculatedValues_ShouldUseCompactFormatAndPreserveCalculationSteps()
        {
            // Arrange - Many calculated values
            var items = new[]
            {
                new { Calc = (10m.As("Base1") * 2m.As("Rate")).As("Calc1") },
                new { Calc = (20m.As("Base2") * 2m.As("Rate")).As("Calc2") },
                new { Calc = (30m.As("Base3") * 2m.As("Rate")).As("Calc3") },
                new { Calc = (40m.As("Base4") * 2m.As("Rate")).As("Calc4") },
                new { Calc = (50m.As("Base5") * 2m.As("Rate")).As("Calc5") }
            };

            // Act
            var total = items.Sum(i => i.Calc);

            // Assert
            Assert.Equal(300m, total.Value); // (10*2) + (20*2) + (30*2) + (40*2) + (50*2) = 20+40+60+80+100 = 300
            Assert.Equal("Sum(Calc, count(5))[300]", total.Caption);
            
            // Should preserve calculation steps from all calculated values
            Assert.Contains("Calc1 = Base1[10] × Rate[2] = 20", total.CalculationSteps);
            Assert.Contains("Calc2 = Base2[20] × Rate[2] = 40", total.CalculationSteps);
            Assert.Contains("Calc3 = Base3[30] × Rate[2] = 60", total.CalculationSteps);
            Assert.Contains("Calc4 = Base4[40] × Rate[2] = 80", total.CalculationSteps);
            Assert.Contains("Calc5 = Base5[50] × Rate[2] = 100", total.CalculationSteps);
        }

        [Fact]
        public void Sum_CombinedWithAddition_ShouldWorkCorrectly()
        {
            // Arrange
            var values1 = new[] { 10m.As("A"), 20m.As("B") };
            var values2 = new[] { 5m.As("C"), 15m.As("D") };
            
            var sum1 = values1.Sum();
            var sum2 = values2.Sum();

            // Act
            var total = sum1 + sum2;

            // Assert
            Assert.Equal(50m, total.Value); // (10+20) + (5+15) = 30 + 20 = 50
            Assert.Equal("A[10] + B[20] + C[5] + D[15]", total.Caption);
        }

        [Fact]
        public void Sum_CombinedWithSubtraction_ShouldWorkCorrectly()
        {
            // Arrange
            var revenues = new[] { 1000m.As("Revenue1"), 1500m.As("Revenue2") };
            var expenses = new[] { 300m.As("Expense1"), 200m.As("Expense2") };
            
            var totalRevenue = revenues.Sum();
            var totalExpenses = expenses.Sum();

            // Act
            var profit = totalRevenue - totalExpenses;

            // Assert
            Assert.Equal(2000m, profit.Value); // (1000+1500) - (300+200) = 2500 - 500 = 2000
            Assert.Equal("Revenue1[1,000] + Revenue2[1,500] - Expense1[300] + Expense2[200]", profit.Caption);
        }

        [Fact]
        public void Sum_CombinedWithMultiplication_ShouldWorkCorrectly()
        {
            // Arrange
            var basePrices = new[] { 100m.As("Base1"), 150m.As("Base2"), 200m.As("Base3") };
            var multiplier = 1.2m.As("TaxMultiplier");
            
            var totalBase = basePrices.Sum();

            // Act
            var totalWithTax = totalBase * multiplier;

            // Assert
            Assert.Equal(540m, totalWithTax.Value); // (100+150+200) * 1.2 = 450 * 1.2 = 540
            Assert.Equal("(Base1[100] + Base2[150] + Base3[200]) × TaxMultiplier[1.2]", totalWithTax.Caption);
        }

        [Fact]
        public void Sum_CombinedWithDivision_ShouldWorkCorrectly()
        {
            // Arrange
            var totalHours = new[] { 8m.As("Monday"), 7.5m.As("Tuesday"), 8m.As("Wednesday") };
            var days = 3m.As("DaysWorked");
            
            var totalTimeWorked = totalHours.Sum();

            // Act
            var averageHours = totalTimeWorked / days;

            // Assert
            Assert.Equal(7.8333333333333333333333333333m, averageHours.Value); // (8+7.5+8) / 3 = 23.5 / 3
            Assert.Equal("(Monday[8] + Tuesday[7.5] + Wednesday[8]) ÷ DaysWorked[3]", averageHours.Caption);
        }

        [Fact]
        public void Sum_WithMultipleSumsInSameExpression_ShouldWorkCorrectly()
        {
            // Arrange
            var q1Sales = new[] { 1000m.As("Jan"), 1200m.As("Feb"), 900m.As("Mar") };
            var q2Sales = new[] { 1100m.As("Apr"), 1300m.As("May"), 1000m.As("Jun") };
            var q3Sales = new[] { 950m.As("Jul"), 1150m.As("Aug"), 1250m.As("Sep") };
            
            var q1Total = q1Sales.Sum();
            var q2Total = q2Sales.Sum(); 
            var q3Total = q3Sales.Sum();

            // Act
            var halfYearTotal = q1Total + q2Total + q3Total;

            // Assert
            Assert.Equal(9850m, halfYearTotal.Value); // 3100 + 3400 + 3350 = 9850
            Assert.Equal("Jan[1,000] + Feb[1,200] + Mar[900] + Apr[1,100] + May[1,300] + Jun[1,000] + Jul[950] + Aug[1,150] + Sep[1,250]", halfYearTotal.Caption);
        }

        [Fact]
        public void Sum_WithParenthesesAndPrecedence_ShouldRespectPrecedence()
        {
            // Arrange
            var items = new[] { 10m.As("Item1"), 20m.As("Item2") };
            var multiplier = 2m.As("Multiplier");
            var bonus = 5m.As("Bonus");
            
            var itemSum = items.Sum();

            // Act - Test precedence: sum * multiplier + bonus should be (sum * multiplier) + bonus
            var result1 = itemSum * multiplier + bonus;
            
            // Act - Test parentheses override: (sum + bonus) * multiplier
            var result2 = (itemSum + bonus) * multiplier;

            // Assert
            Assert.Equal(65m, result1.Value); // (10+20) * 2 + 5 = 30 * 2 + 5 = 60 + 5 = 65
            Assert.Equal("(Item1[10] + Item2[20]) × Multiplier[2] + Bonus[5]", result1.Caption);
            
            Assert.Equal(70m, result2.Value); // ((10+20) + 5) * 2 = (30 + 5) * 2 = 35 * 2 = 70
            Assert.Equal("(Item1[10] + Item2[20] + Bonus[5]) × Multiplier[2]", result2.Caption);
        }

        [Fact]
        public void Sum_CompactFormat_CombinedWithOperators_ShouldWorkCorrectly()
        {
            // Arrange - More than 3 items to trigger compact format
            var sales = new[]
            {
                new { Amount = 100m.As("Sale1") },
                new { Amount = 150m.As("Sale2") },
                new { Amount = 200m.As("Sale3") },
                new { Amount = 120m.As("Sale4") },
                new { Amount = 180m.As("Sale5") }
            };
            
            var commission = 0.1m.As("CommissionRate");
            var bonus = 50m.As("Bonus");
            
            var totalSales = sales.Sum(s => s.Amount);

            // Act
            var totalEarnings = totalSales * commission + bonus;

            // Assert
            Assert.Equal(125m, totalEarnings.Value); // 750 * 0.1 + 50 = 75 + 50 = 125
            Assert.Equal("(Sum(Sale, count(5))[750]) × CommissionRate[0.1] + Bonus[50]", totalEarnings.Caption);
        }

        [Fact]
        public void Sum_CombinedWithWrappedResults_ShouldShowCalculationSteps()
        {
            // Arrange
            var prices = new[] { 100m.As("Price1"), 200m.As("Price2") };
            var tax = 0.18m.As("TaxRate");
            
            var subtotal = prices.Sum().As("Subtotal");
            var taxAmount = (subtotal * tax).As("TaxAmount");

            // Act
            var total = (subtotal + taxAmount).As("Total");

            // Assert
            Assert.Equal(354m, total.Value); // 300 + (300 * 0.18) = 300 + 54 = 354
            Assert.Equal("Total", total.Caption);
            
            // Check calculation steps are properly combined
            var expectedSteps = """
                Subtotal = Price1[100] + Price2[200] = 300

                TaxAmount = Subtotal[300] × TaxRate[0.18] = 54

                Total = Subtotal[300] + TaxAmount[54] = 354
                """;
                
            Assert.Equal(expectedSteps, total.FinalCalculationSteps);
        }

        [Fact]
        public void Sum_NestedWithComplexArithmetic_ShouldMaintainCorrectPrecedence()
        {
            // Arrange
            var groupA = new[] { 10m.As("A1"), 15m.As("A2") };
            var groupB = new[] { 20m.As("B1"), 25m.As("B2") };
            var groupC = new[] { 5m.As("C1"), 10m.As("C2") };
            
            var sumA = groupA.Sum();
            var sumB = groupB.Sum();
            var sumC = groupC.Sum();

            // Act - Complex expression: (sumA + sumB) * sumC - 50
            var result = (sumA + sumB) * sumC - 50m.As("Deduction");

            // Assert
            // sumA = 10 + 15 = 25
            // sumB = 20 + 25 = 45  
            // sumC = 5 + 10 = 15
            // (25 + 45) * 15 - 50 = 70 * 15 - 50 = 1050 - 50 = 1000
            Assert.Equal(1000m, result.Value);
            Assert.Equal("(A1[10] + A2[15] + B1[20] + B2[25]) × (C1[5] + C2[10]) - Deduction[50]", result.Caption);
        }
    }
}