using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    public class ComplexScenarioTests
    {
        #region Same Variable Multiple Times Tests

        [Fact]
        public void SameVariable_UsedMultipleTimes_InAdditionAndMultiplication()
        {
            // Test case: x + x * x = x + (x × x)
            // This tests variable reuse in different operations with precedence
            var x = 5m.As("x");

            var result = x + x * x;

            Assert.Equal(30m, result.Value); // 5 + (5 × 5) = 5 + 25 = 30
            Assert.Equal("x[5] + x[5] × x[5]", result.Caption);
        }

        [Fact]
        public void SameVariable_InParentheses_MultipleOccurrences()
        {
            // Test case: (a + a) * (a - a) = (a + a) × (a - a)
            // This tests variable reuse within parentheses - interesting case where result should be 0
            var a = 7m.As("a");

            var result = (a + a) * (a - a);

            Assert.Equal(0m, result.Value); // (7 + 7) × (7 - 7) = 14 × 0 = 0
            Assert.Equal("(a[7] + a[7]) × (a[7] - a[7])", result.Caption);
        }

        [Fact]
        public void SameVariable_ThreeTimesInMultiplication()
        {
            // Test case: b * b * b = b³
            var b = 3m.As("b");

            var result = b * b * b;

            Assert.Equal(27m, result.Value); // 3 × 3 × 3 = 27
            Assert.Equal("b[3] × b[3] × b[3]", result.Caption);
        }

        [Fact]
        public void SameVariable_UsedMultipleTimesInSameExpression()
        {
            // Test where the same base variable is used multiple times in one expression
            var rate = 0.05m.As("rate");
            var principal = 1000m.As("principal");
            var fee = 50m.As("fee");

            // Use rate multiple times in the SAME expression
            var total = (rate * principal + rate * fee).As("TotalInterest");

            Assert.Equal(52.5m, total.Value); // 0.05 × 1000 + 0.05 × 50 = 50 + 2.5 = 52.5
            Assert.Equal("TotalInterest", total.Caption);

            var expectedSteps =
"""
TotalInterest = rate[0.05] × principal[1,000] + rate[0.05] × fee[50] = 52.5
""";
            Assert.Equal(expectedSteps, total.FinalCalculationSteps);
        }

        [Fact]
        public void SameWrappedVariable_UsedMultipleTimesInComplexExpression()
        {
            // Test wrapped variables used multiple times in a complex expression
            var discount = 0.1m.As("discount");
            var price1 = 100m.As("price1");
            var price2 = 200m.As("price2");

            var discountAmount1 = (price1 * discount).As("Discount1");
            var discountAmount2 = (price2 * discount).As("Discount2");

            // Use both discount amounts in final calculation
            var totalSavings = (discountAmount1 + discountAmount2).As("TotalSavings");

            Assert.Equal(10m, discountAmount1.Value); // 100 × 0.1 = 10
            Assert.Equal(20m, discountAmount2.Value); // 200 × 0.1 = 20
            Assert.Equal(30m, totalSavings.Value); // 10 + 20 = 30
            Assert.Equal("TotalSavings", totalSavings.Caption);

            var expectedSteps =
"""
Discount1 = price1[100] × discount[0.1] = 10

Discount2 = price2[200] × discount[0.1] = 20

TotalSavings = Discount1[10] + Discount2[20] = 30
""";
            Assert.Equal(expectedSteps, totalSavings.FinalCalculationSteps);
        }

        #endregion

        #region Strange Algebraic Cases

        [Fact]
        public void AlgebraicCase_APlusBMinusA_SimplifiesToB()
        {
            // Test case: a + b - a, which algebraically simplifies to b
            // But we should still show the full expression with variable substitution
            var a = 10m.As("a");
            var b = 7m.As("b");

            var result = (a + b - a).As("Result");

            Assert.Equal(7m, result.Value); // 10 + 7 - 10 = 7 (equals b)
            Assert.Equal("Result", result.Caption);

            var expectedSteps =
"""
Result = a[10] + b[7] - a[10] = 7
""";
            Assert.Equal(expectedSteps, result.FinalCalculationSteps);
        }

        [Fact]
        public void AlgebraicCase_XTimesYDividedByX_SimplifiesToY()
        {
            // Test case: x * y / x, which should equal y
            var x = 8m.As("x");
            var y = 5m.As("y");

            var result = (x * y / x).As("Simplified");

            Assert.Equal(5m, result.Value); // (8 × 5) ÷ 8 = 40 ÷ 8 = 5 (equals y)
            Assert.Equal("Simplified", result.Caption);

            var expectedSteps =
"""
Simplified = x[8] × y[5] ÷ x[8] = 5
""";
            Assert.Equal(expectedSteps, result.FinalCalculationSteps);
        }

        [Fact]
        public void AlgebraicCase_AMinusAResultsInZero()
        {
            // Test case: a - a = 0
            var a = 15m.As("a");

            var result = (a - a).As("Zero");

            Assert.Equal(0m, result.Value);
            Assert.Equal("Zero", result.Caption);

            var expectedSteps =
"""
Zero = a[15] - a[15] = 0
""";
            Assert.Equal(expectedSteps, result.FinalCalculationSteps);
        }

        [Fact]
        public void AlgebraicCase_ComplexCancellation()
        {
            // Test case: (a + b) - (a - b) = 2b
            var a = 12m.As("a");
            var b = 8m.As("b");

            var result = ((a + b) - (a - b)).As("TwoB");

            Assert.Equal(16m, result.Value); // (12 + 8) - (12 - 8) = 20 - 4 = 16 = 2×8
            Assert.Equal("TwoB", result.Caption);

            var expectedSteps =
"""
TwoB = a[12] + b[8] - a[12] - b[8] = 16
""";
            Assert.Equal(expectedSteps, result.FinalCalculationSteps);
        }

        [Fact]
        public void AlgebraicCase_VariableTimesZero()
        {
            // Test case: x * 0 = 0 (any variable times zero)
            var x = 42m.As("x");
            var zero = 0m.As("zero");

            var result = (x * zero).As("AlwaysZero");

            Assert.Equal(0m, result.Value);
            Assert.Equal("AlwaysZero", result.Caption);

            var expectedSteps =
"""
AlwaysZero = x[42] × zero[0] = 0
""";
            Assert.Equal(expectedSteps, result.FinalCalculationSteps);
        }

        [Fact]
        public void AlgebraicCase_XPlusXMinusX_SimplifiesToX()
        {
            // Test case: x + x - x = x (algebraically simplifies but we show full substitution)
            var x = 25m.As("x");

            var result = (x + x - x).As("BackToX");

            Assert.Equal(25m, result.Value); // 25 + 25 - 25 = 25 (equals x)
            Assert.Equal("BackToX", result.Caption);

            var expectedSteps =
"""
BackToX = x[25] + x[25] - x[25] = 25
""";
            Assert.Equal(expectedSteps, result.FinalCalculationSteps);
        }

        #endregion

        #region Multi-Level Dependency Tests

        [Fact]
        public void MultiLevel_SimpleChain_ProperDependencyOrdering()
        {
            // Test: A → B → C → Final (simple linear dependency chain)
            var baseValue = 10m.As("Base");

            var stepA = (baseValue * 2m.As("Factor1")).As("StepA");
            var stepB = (stepA + 5m.As("Adjustment")).As("StepB");
            var final = (stepB * 3m.As("Multiplier")).As("Final");

            Assert.Equal(20m, stepA.Value); // 10 × 2 = 20
            Assert.Equal(25m, stepB.Value); // 20 + 5 = 25
            Assert.Equal(75m, final.Value); // 25 × 3 = 75
            Assert.Equal("Final", final.Caption);

            var expectedSteps =
"""
StepA = Base[10] × Factor1[2] = 20

StepB = StepA[20] + Adjustment[5] = 25

Final = StepB[25] × Multiplier[3] = 75
""";
            Assert.Equal(expectedSteps, final.FinalCalculationSteps);
        }

        [Fact]
        public void MultiLevel_DiamondPattern_TwoBranchesConverge()
        {
            // Test: A → B,C → D (diamond dependency pattern)
            var root = 100m.As("Root");

            var branchB = (root - 20m.As("DeductionB")).As("BranchB");
            var branchC = (root * 0.5m.As("FactorC")).As("BranchC");
            var convergence = (branchB + branchC).As("Convergence");

            Assert.Equal(80m, branchB.Value); // 100 - 20 = 80
            Assert.Equal(50m, branchC.Value); // 100 × 0.5 = 50
            Assert.Equal(130m, convergence.Value); // 80 + 50 = 130
            Assert.Equal("Convergence", convergence.Caption);

            var expectedSteps =
"""
BranchB = Root[100] - DeductionB[20] = 80

BranchC = Root[100] × FactorC[0.5] = 50

Convergence = BranchB[80] + BranchC[50] = 130
""";
            Assert.Equal(expectedSteps, convergence.FinalCalculationSteps);
        }

        [Fact]
        public void MultiLevel_ComplexBusinessLogic_FullDependencyTree()
        {
            // Test complex business pricing logic with multiple dependency levels
            var basePrice = 200m.As("BasePrice");
            var discountRate = 0.15m.As("DiscountRate");
            var taxRate = 0.08m.As("TaxRate");
            var shippingRate = 0.05m.As("ShippingRate");

            // Level 1: Basic calculations
            var discount = (basePrice * discountRate).As("Discount");
            var discountedPrice = (basePrice - discount).As("DiscountedPrice");

            // Level 2: Tax and shipping based on discounted price
            var tax = (discountedPrice * taxRate).As("Tax");
            var shipping = (discountedPrice * shippingRate).As("Shipping");

            // Level 3: Final total
            var finalTotal = (discountedPrice + tax + shipping).As("FinalTotal");

            Assert.Equal(30m, discount.Value); // 200 × 0.15 = 30
            Assert.Equal(170m, discountedPrice.Value); // 200 - 30 = 170
            Assert.Equal(13.6m, tax.Value); // 170 × 0.08 = 13.6
            Assert.Equal(8.5m, shipping.Value); // 170 × 0.05 = 8.5
            Assert.Equal(192.1m, finalTotal.Value); // 170 + 13.6 + 8.5 = 192.1

            var expectedSteps =
"""
Discount = BasePrice[200] × DiscountRate[0.15] = 30

DiscountedPrice = BasePrice[200] - Discount[30] = 170

Tax = DiscountedPrice[170] × TaxRate[0.08] = 13.6

Shipping = DiscountedPrice[170] × ShippingRate[0.05] = 8.5

FinalTotal = DiscountedPrice[170] + Tax[13.6] + Shipping[8.5] = 192.1
""";
            Assert.Equal(expectedSteps, finalTotal.FinalCalculationSteps);
        }

        [Fact]
        public void MultiLevel_DeepNesting_SixLevelsDeep()
        {
            // Test deep nesting with 6 levels of dependencies
            var base1 = 5m.As("Base");

            var level1 = (base1 * 2m.As("L1Factor")).As("Level1");
            var level2 = (level1 + 3m.As("L2Add")).As("Level2");
            var level3 = (level2 * 1.5m.As("L3Mult")).As("Level3");
            var level4 = (level3 - 2m.As("L4Sub")).As("Level4");
            var level5 = (level4 / 2m.As("L5Div")).As("Level5");
            var level6 = (level5 + 1m.As("L6Final")).As("Level6");

            // Calculation: ((((5×2)+3)×1.5)-2)÷2)+1 = (((10+3)×1.5)-2)÷2)+1 = ((13×1.5)-2)÷2)+1 = (19.5-2)÷2)+1 = 17.5÷2)+1 = 8.75+1 = 9.75
            Assert.Equal(10m, level1.Value); // 5 × 2 = 10
            Assert.Equal(13m, level2.Value); // 10 + 3 = 13
            Assert.Equal(19.5m, level3.Value); // 13 × 1.5 = 19.5
            Assert.Equal(17.5m, level4.Value); // 19.5 - 2 = 17.5
            Assert.Equal(8.75m, level5.Value); // 17.5 ÷ 2 = 8.75
            Assert.Equal(9.75m, level6.Value); // 8.75 + 1 = 9.75

            var expectedSteps =
"""
Level1 = Base[5] × L1Factor[2] = 10

Level2 = Level1[10] + L2Add[3] = 13

Level3 = Level2[13] × L3Mult[1.5] = 19.5

Level4 = Level3[19.5] - L4Sub[2] = 17.5

Level5 = Level4[17.5] ÷ L5Div[2] = 8.75

Level6 = Level5[8.75] + L6Final[1] = 9.75
""";
            Assert.Equal(expectedSteps, level6.FinalCalculationSteps);
        }

        [Fact]
        public void MultiLevel_ReusedIntermediates_InMultipleCalculations()
        {
            // Test where same intermediate is used in multiple final calculations
            var principal = 1000m.As("Principal");
            var rate = 0.06m.As("Rate");
            var time = 2m.As("Time");

            var interest = (principal * rate * time).As("Interest");

            // Use interest in multiple calculations
            var totalWithInterest = (principal + interest).As("TotalWithInterest");
            var interestTax = (interest * 0.25m.As("TaxRate")).As("InterestTax");
            var netTotal = (totalWithInterest - interestTax).As("NetTotal");

            Assert.Equal(120m, interest.Value); // 1000 × 0.06 × 2 = 120
            Assert.Equal(1120m, totalWithInterest.Value); // 1000 + 120 = 1120
            Assert.Equal(30m, interestTax.Value); // 120 × 0.25 = 30
            Assert.Equal(1090m, netTotal.Value); // 1120 - 30 = 1090

            var expectedSteps =
"""
Interest = Principal[1,000] × Rate[0.06] × Time[2] = 120

TotalWithInterest = Principal[1,000] + Interest[120] = 1,120

InterestTax = Interest[120] × TaxRate[0.25] = 30

NetTotal = TotalWithInterest[1,120] - InterestTax[30] = 1,090
""";
            Assert.Equal(expectedSteps, netTotal.FinalCalculationSteps);
        }

        [Fact]
        public void MultiLevel_SameIntermediateUsedTwiceInFinalCalculation()
        {
            // Test where the same intermediate value appears twice in final calculation
            var baseValue = 10m.As("Base");
            var factor = 3m.As("Factor");

            var intermediate = (baseValue * factor).As("Intermediate");

            // Use intermediate twice in final calculation: intermediate + intermediate
            var doubleIntermediate = (intermediate + intermediate).As("DoubleValue");

            Assert.Equal(30m, intermediate.Value); // 10 × 3 = 30
            Assert.Equal(60m, doubleIntermediate.Value); // 30 + 30 = 60

            var expectedSteps =
"""
Intermediate = Base[10] × Factor[3] = 30

DoubleValue = Intermediate[30] + Intermediate[30] = 60
""";
            Assert.Equal(expectedSteps, doubleIntermediate.FinalCalculationSteps);
        }

        #endregion

        #region Real-World Complex Scenarios

        [Fact]
        public void ComplexFinancial_LoanCalculationWithMultipleIntermediates()
        {
            // Complex loan calculation with multiple rates and fees
            var loanAmount = 50000m.As("LoanAmount");
            var interestRate = 0.045m.As("InterestRate");
            var originationFeeRate = 0.01m.As("OriginationFeeRate");
            var insuranceRate = 0.005m.As("InsuranceRate");
            var years = 5m.As("Years");

            var originationFee = (loanAmount * originationFeeRate).As("OriginationFee");
            var annualInsurance = (loanAmount * insuranceRate).As("AnnualInsurance");
            var totalInsurance = (annualInsurance * years).As("TotalInsurance");
            var principalWithFees = (loanAmount + originationFee + totalInsurance).As("PrincipalWithFees");
            var totalInterest = (principalWithFees * interestRate * years).As("TotalInterest");
            var totalPayment = (principalWithFees + totalInterest).As("TotalPayment");

            Assert.Equal(500m, originationFee.Value); // 50000 × 0.01 = 500
            Assert.Equal(250m, annualInsurance.Value); // 50000 × 0.005 = 250
            Assert.Equal(1250m, totalInsurance.Value); // 250 × 5 = 1250
            Assert.Equal(51750m, principalWithFees.Value); // 50000 + 500 + 1250 = 51750
            Assert.Equal(11643.75m, totalInterest.Value); // 51750 × 0.045 × 5 = 11643.75
            Assert.Equal(63393.75m, totalPayment.Value); // 51750 + 11643.75 = 63393.75

            // Test calculation steps count to identify duplications
            var finalStepsText = totalPayment.FinalCalculationSteps;
            var stepLines = finalStepsText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(6, stepLines.Length); // Should have exactly 6 unique calculation steps

            var expectedSteps =
"""
OriginationFee = LoanAmount[50,000] × OriginationFeeRate[0.01] = 500

AnnualInsurance = LoanAmount[50,000] × InsuranceRate[0.01] = 250

TotalInsurance = AnnualInsurance[250] × Years[5] = 1,250

PrincipalWithFees = LoanAmount[50,000] + OriginationFee[500] + TotalInsurance[1,250] = 51,750

TotalInterest = PrincipalWithFees[51,750] × InterestRate[0.05] × Years[5] = 11,643.75

TotalPayment = PrincipalWithFees[51,750] + TotalInterest[11,643.75] = 63,393.75
""";
            Assert.Equal(expectedSteps, totalPayment.FinalCalculationSteps);
        }

        [Fact]
        public void ComplexEngineering_VolumeAndDensityCalculations()
        {
            // Engineering calculation with geometric and material properties
            var length = 10m.As("Length");
            var width = 5m.As("Width");
            var height = 3m.As("Height");
            var materialDensity = 7.85m.As("SteelDensity"); // kg/m³ (simplified)
            var safetyFactor = 1.5m.As("SafetyFactor");
            var costPerKg = 2.5m.As("CostPerKg");

            var area = (length * width).As("Area");
            var volume = (area * height).As("Volume");
            var mass = (volume * materialDensity).As("Mass");
            var designLoad = (mass * safetyFactor).As("DesignLoad");
            var totalCost = (mass * costPerKg).As("TotalCost");

            Assert.Equal(50m, area.Value); // 10 × 5 = 50
            Assert.Equal(150m, volume.Value); // 50 × 3 = 150
            Assert.Equal(1177.5m, mass.Value); // 150 × 7.85 = 1177.5
            Assert.Equal(1766.25m, designLoad.Value); // 1177.5 × 1.5 = 1766.25
            Assert.Equal(2943.75m, totalCost.Value); // 1177.5 × 2.5 = 2943.75

            var expectedSteps =
"""
Area = Length[10] × Width[5] = 50

Volume = Area[50] × Height[3] = 150

Mass = Volume[150] × SteelDensity[7.85] = 1,177.5

TotalCost = Mass[1,177.5] × CostPerKg[2.5] = 2,943.75
""";
            Assert.Equal(expectedSteps, totalCost.FinalCalculationSteps);
        }

        [Fact]
        public void ComplexBusiness_PricingWithTieredDiscounts()
        {
            // Complex business logic with tiered discounts and multiple customer types
            var basePrice = 1000m.As("BasePrice");
            var quantity = 15m.As("Quantity");
            var volumeDiscountRate = 0.1m.As("VolumeDiscountRate"); // 10% for >10 items
            var loyaltyDiscountRate = 0.05m.As("LoyaltyDiscountRate"); // 5% loyalty discount
            var rushOrderSurcharge = 0.15m.As("RushOrderSurcharge"); // 15% rush fee
            var taxRate = 0.0875m.As("TaxRate"); // 8.75% tax

            var subtotal = (basePrice * quantity).As("Subtotal");
            var volumeDiscount = (subtotal * volumeDiscountRate).As("VolumeDiscount");
            var afterVolumeDiscount = (subtotal - volumeDiscount).As("AfterVolumeDiscount");
            var loyaltyDiscount = (afterVolumeDiscount * loyaltyDiscountRate).As("LoyaltyDiscount");
            var afterLoyaltyDiscount = (afterVolumeDiscount - loyaltyDiscount).As("AfterLoyaltyDiscount");
            var rushFee = (afterLoyaltyDiscount * rushOrderSurcharge).As("RushFee");
            var afterRushFee = (afterLoyaltyDiscount + rushFee).As("AfterRushFee");
            var tax = (afterRushFee * taxRate).As("Tax");
            var finalPrice = (afterRushFee + tax).As("FinalPrice");

            Assert.Equal(15000m, subtotal.Value); // 1000 × 15 = 15000
            Assert.Equal(1500m, volumeDiscount.Value); // 15000 × 0.1 = 1500
            Assert.Equal(13500m, afterVolumeDiscount.Value); // 15000 - 1500 = 13500
            Assert.Equal(675m, loyaltyDiscount.Value); // 13500 × 0.05 = 675
            Assert.Equal(12825m, afterLoyaltyDiscount.Value); // 13500 - 675 = 12825
            Assert.Equal(1923.75m, rushFee.Value); // 12825 × 0.15 = 1923.75
            Assert.Equal(14748.75m, afterRushFee.Value); // 12825 + 1923.75 = 14748.75
            Assert.Equal(1290.515625m, tax.Value); // 14748.75 × 0.0875 = 1290.515625
            Assert.Equal(16039.265625m, finalPrice.Value); // 14748.75 + 1290.515625 = 16039.265625

            // Test calculation steps count to identify duplications
            var finalStepsText = finalPrice.FinalCalculationSteps;
            var stepLines = finalStepsText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(9, stepLines.Length); // Should have exactly 9 unique calculation steps

            var expectedSteps =
"""
Subtotal = BasePrice[1,000] × Quantity[15] = 15,000

VolumeDiscount = Subtotal[15,000] × VolumeDiscountRate[0.1] = 1,500

AfterVolumeDiscount = Subtotal[15,000] - VolumeDiscount[1,500] = 13,500

LoyaltyDiscount = AfterVolumeDiscount[13,500] × LoyaltyDiscountRate[0.05] = 675

AfterLoyaltyDiscount = AfterVolumeDiscount[13,500] - LoyaltyDiscount[675] = 12,825

RushFee = AfterLoyaltyDiscount[12,825] × RushOrderSurcharge[0.15] = 1,923.75

AfterRushFee = AfterLoyaltyDiscount[12,825] + RushFee[1,923.75] = 14,748.75

Tax = AfterRushFee[14,748.75] × TaxRate[0.09] = 1,290.52

FinalPrice = AfterRushFee[14,748.75] + Tax[1,290.52] = 16,039.27
""";
            Assert.Equal(expectedSteps, finalPrice.FinalCalculationSteps);
        }

        [Fact]
        public void GeorgianLanguage_ComplexCalculationWithMultipleIntermediates()
        {
            // Complex calculation using Georgian language similar to existing tests
            var basePrice = 100m.As("საბაზო ღირებულება");
            var discount = 25m.As("ფასდაკლება");
            var taxRate = 0.18m.As("დღგ");
            var serviceFee = 15m.As("მომსახურების გადასახადი");
            var multiplier = 1.2m.As("მრავალი");

            var discountedPrice = (basePrice - discount).As("ფასდაკლების შემდეგ");
            var tax = (discountedPrice * taxRate).As("დღგ-ის რაოდენობა");
            var withTax = (discountedPrice + tax).As("დღგ-ით");
            var withService = (withTax + serviceFee).As("მომსახურებით");
            var finalAmount = (withService * multiplier).As("საბოლოო თანხა");

            Assert.Equal(75m, discountedPrice.Value); // 100 - 25 = 75
            Assert.Equal(13.5m, tax.Value); // 75 × 0.18 = 13.5
            Assert.Equal(88.5m, withTax.Value); // 75 + 13.5 = 88.5
            Assert.Equal(103.5m, withService.Value); // 88.5 + 15 = 103.5
            Assert.Equal(124.2m, finalAmount.Value); // 103.5 × 1.2 = 124.2

            // Test calculation steps count to verify no duplications
            var finalStepsText = finalAmount.FinalCalculationSteps;
            var stepLines = finalStepsText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(5, stepLines.Length); // Should have exactly 5 unique calculation steps

            var expectedSteps =
"""
ფასდაკლების შემდეგ = საბაზო ღირებულება[100] - ფასდაკლება[25] = 75

დღგ-ის რაოდენობა = ფასდაკლების შემდეგ[75] × დღგ[0.18] = 13.5

დღგ-ით = ფასდაკლების შემდეგ[75] + დღგ-ის რაოდენობა[13.5] = 88.5

მომსახურებით = დღგ-ით[88.5] + მომსახურების გადასახადი[15] = 103.5

საბოლოო თანხა = მომსახურებით[103.5] × მრავალი[1.2] = 124.2
""";
            Assert.Equal(expectedSteps, finalAmount.FinalCalculationSteps);
        }

        [Fact]
        public void NonWrappedVariable_DoesNotHaveFinalCalculationSteps()
        {
            // Test that non-wrapped variables don't support FinalCalculationSteps
            var price = 100m.As("price");
            var tax = 18m.As("tax");
            
            // This is NOT wrapped with .As(), so should not have FinalCalculationSteps
            var total = price + tax;
            
            Assert.Equal(118m, total.Value);
            Assert.Equal("price[100] + tax[18]", total.Caption);
            
            // total.FinalCalculationSteps should not be available/compilable
            // This test demonstrates the new API design where only .As() wrapped variables 
            // can show final calculation steps
        }

        [Fact]
        public void OnlyWrappedVariables_CanShowFinalCalculationSteps()
        {
            // Test that only variables with .As() can use FinalCalculationSteps
            var baseAmount = 100m.As("BaseAmount");
            var rate = 0.1m.As("Rate");
            
            // Non-wrapped intermediate calculation
            var intermediate = baseAmount * rate;
            Assert.Equal(10m, intermediate.Value);
            Assert.Equal("BaseAmount[100] × Rate[0.1]", intermediate.Caption);
            // intermediate.FinalCalculationSteps would not be available
            
            // Wrapped final calculation
            var finalResult = (intermediate + baseAmount).As("FinalResult");
            Assert.Equal(110m, finalResult.Value);
            Assert.Equal("FinalResult", finalResult.Caption);
            
            var expectedSteps = 
"""
FinalResult = BaseAmount[100] × Rate[0.1] + BaseAmount[100] = 110
""";
            Assert.Equal(expectedSteps, finalResult.FinalCalculationSteps);
        }

        #endregion
    }
}