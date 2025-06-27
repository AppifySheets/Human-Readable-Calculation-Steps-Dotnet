using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    public class ComplexCalculationTests
    {
        [Fact]
        public void ComplexCalculation_WithWrappedAndNonWrappedValues_ShouldShowCorrectFinalCalculationSteps()
        {
            var basePrice = 100m.As("საბაზო ფასი");
            var taxRate = 0.18m.As("დღგ");
            var discount = 15m.As("ფასდაკლება");

            var tax = (basePrice * taxRate).As("TaxValue");
            var discountedPrice = basePrice - discount;
            var finalPrice = (discountedPrice + tax) * 120.0m.As("ასოცი");

            var wrappedFinalPrice = finalPrice.As("FinalValue");
            var actualOutput = wrappedFinalPrice.FinalCalculationSteps;
            
            Assert.Equal(
"""
TaxValue =
  საბაზო ფასი[100] 
× დღგ[0.18]
= 18

FinalValue =
  (  საბაზო ფასი[100]
   - ფასდაკლება[15]
   + TaxValue[18]
  )
× ასოცი[120]
= 12,360
""", actualOutput);
        }
        
        [Fact]
        public void ComplexCalculation_WithWrappedAndNonWrappedValues_ShouldShowCorrectFinalCalculationSteps2()
        {
            var basePrice = 100m.As("საბაზო ფასი");
            var taxRate = 0.18m.As("დღგ");
            var discount = 15m.As("ფასდაკლება");

            var tax = (basePrice * taxRate).As("TaxValue");
	
            //tax.Dump();
            //
            //return;
	
            var discountedPrice = (basePrice - discount).As("DiscountedPrice");
	
            var someValue = (discountedPrice * 55.233m.As("SomeValue")).As("SomeValueResult");
		
            var finalPrice = (someValue * (discountedPrice + tax) * 120.0m.As("ასოცი")).As("FinalValue");
            // var wrappedFinalPrice = finalPrice.As("FinalValue");
            var actualOutput = finalPrice.FinalCalculationSteps;
            
            Assert.Equal(
                """
                DiscountedPrice = საბაზო ფასი[100] - ფასდაკლება[15] = 85

                SomeValueResult = DiscountedPrice[85] × SomeValue[55.23] = 4,694.8

                TaxValue = საბაზო ფასი[100] × დღგ[0.18] = 18

                FinalValue = SomeValueResult[4,694.8] × (DiscountedPrice[85] + TaxValue[18]) × ასოცი[120] = 58,027,789.8
                """, actualOutput);
        }
    }
}