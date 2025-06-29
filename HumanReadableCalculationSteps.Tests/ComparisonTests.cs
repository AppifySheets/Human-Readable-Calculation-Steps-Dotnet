using Xunit;

namespace HumanReadableCalculationSteps.Tests
{
    public class ComparisonTests
    {
        [Fact]
        public void GreaterThan_WhenLeftIsGreater_ShouldReturnTrue()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a > b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void GreaterThan_WhenLeftIsSmaller_ShouldReturnFalse()
        {
            var a = 5m.As("a");
            var b = 10m.As("b");
            
            var result = a > b;
            
            Assert.False(result);
        }
        
        [Fact]
        public void GreaterThan_WhenEqual_ShouldReturnFalse()
        {
            var a = 10m.As("a");
            var b = 10m.As("b");
            
            var result = a > b;
            
            Assert.False(result);
        }
        
        [Fact]
        public void LessThan_WhenLeftIsSmaller_ShouldReturnTrue()
        {
            var a = 5m.As("a");
            var b = 10m.As("b");
            
            var result = a < b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void LessThan_WhenLeftIsGreater_ShouldReturnFalse()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a < b;
            
            Assert.False(result);
        }
        
        [Fact]
        public void LessThan_WhenEqual_ShouldReturnFalse()
        {
            var a = 10m.As("a");
            var b = 10m.As("b");
            
            var result = a < b;
            
            Assert.False(result);
        }
        
        [Fact]
        public void GreaterThanOrEqual_WhenLeftIsGreater_ShouldReturnTrue()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a >= b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void GreaterThanOrEqual_WhenEqual_ShouldReturnTrue()
        {
            var a = 10m.As("a");
            var b = 10m.As("b");
            
            var result = a >= b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void GreaterThanOrEqual_WhenLeftIsSmaller_ShouldReturnFalse()
        {
            var a = 5m.As("a");
            var b = 10m.As("b");
            
            var result = a >= b;
            
            Assert.False(result);
        }
        
        [Fact]
        public void LessThanOrEqual_WhenLeftIsSmaller_ShouldReturnTrue()
        {
            var a = 5m.As("a");
            var b = 10m.As("b");
            
            var result = a <= b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void LessThanOrEqual_WhenEqual_ShouldReturnTrue()
        {
            var a = 10m.As("a");
            var b = 10m.As("b");
            
            var result = a <= b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void LessThanOrEqual_WhenLeftIsGreater_ShouldReturnFalse()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a <= b;
            
            Assert.False(result);
        }
        
        [Fact]
        public void Equality_WhenEqual_ShouldReturnTrue()
        {
            var a = 10m.As("a");
            var b = 10m.As("b");
            
            var result = a == b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void Equality_WhenNotEqual_ShouldReturnFalse()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a == b;
            
            Assert.False(result);
        }
        
        [Fact]
        public void Inequality_WhenNotEqual_ShouldReturnTrue()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            
            var result = a != b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void Inequality_WhenEqual_ShouldReturnFalse()
        {
            var a = 10m.As("a");
            var b = 10m.As("b");
            
            var result = a != b;
            
            Assert.False(result);
        }
        
        [Fact]
        public void Comparison_WithComputedValues_ShouldWork()
        {
            var a = 10m.As("a");
            var b = 5m.As("b");
            var sum = (a + b).As("sum");
            var product = (a * b).As("product");
            
            var result = sum < product;
            
            Assert.True(result);
        }
        
        [Fact]
        public void Comparison_WithDecimalValues_ShouldWork()
        {
            var a = 10.5m.As("a");
            var b = 10.25m.As("b");
            
            var result = a > b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void Comparison_WithNegativeValues_ShouldWork()
        {
            var a = (-5m).As("a");
            var b = (-10m).As("b");
            
            var result = a > b;
            
            Assert.True(result);
        }
        
        [Fact]
        public void Comparison_WithZero_ShouldWork()
        {
            var a = 0m.As("a");
            var b = 5m.As("b");
            
            var result = a < b;
            
            Assert.True(result);
        }
        
        // Decimal comparison tests
        [Fact]
        public void GreaterThan_ValueWithCaptionVsDecimal_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(a > 5m);
            Assert.False(a > 15m);
            Assert.False(a > 10m);
        }
        
        [Fact]
        public void GreaterThan_DecimalVsValueWithCaption_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(15m > a);
            Assert.False(5m > a);
            Assert.False(10m > a);
        }
        
        [Fact]
        public void LessThan_ValueWithCaptionVsDecimal_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(a < 15m);
            Assert.False(a < 5m);
            Assert.False(a < 10m);
        }
        
        [Fact]
        public void LessThan_DecimalVsValueWithCaption_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(5m < a);
            Assert.False(15m < a);
            Assert.False(10m < a);
        }
        
        [Fact]
        public void GreaterThanOrEqual_ValueWithCaptionVsDecimal_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(a >= 5m);
            Assert.True(a >= 10m);
            Assert.False(a >= 15m);
        }
        
        [Fact]
        public void GreaterThanOrEqual_DecimalVsValueWithCaption_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(15m >= a);
            Assert.True(10m >= a);
            Assert.False(5m >= a);
        }
        
        [Fact]
        public void LessThanOrEqual_ValueWithCaptionVsDecimal_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(a <= 15m);
            Assert.True(a <= 10m);
            Assert.False(a <= 5m);
        }
        
        [Fact]
        public void LessThanOrEqual_DecimalVsValueWithCaption_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(5m <= a);
            Assert.True(10m <= a);
            Assert.False(15m <= a);
        }
        
        [Fact]
        public void Equality_ValueWithCaptionVsDecimal_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(a == 10m);
            Assert.False(a == 5m);
            Assert.False(a == 15m);
        }
        
        [Fact]
        public void Equality_DecimalVsValueWithCaption_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(10m == a);
            Assert.False(5m == a);
            Assert.False(15m == a);
        }
        
        [Fact]
        public void Inequality_ValueWithCaptionVsDecimal_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(a != 5m);
            Assert.True(a != 15m);
            Assert.False(a != 10m);
        }
        
        [Fact]
        public void Inequality_DecimalVsValueWithCaption_ShouldWork()
        {
            var a = 10m.As("a");
            
            Assert.True(5m != a);
            Assert.True(15m != a);
            Assert.False(10m != a);
        }
        
        [Fact]
        public void Comparison_WithIntegers_ShouldWork()
        {
            var a = 10m.As("a");
            
            // Integer comparisons should work via implicit conversion to decimal
            Assert.True(a > 5);
            Assert.True(a < 15);
            Assert.True(a >= 10);
            Assert.True(a <= 10);
            Assert.True(a == 10);
            Assert.True(a != 5);
            
            Assert.True(15 > a);
            Assert.True(5 < a);
            Assert.True(10 >= a);
            Assert.True(10 <= a);
            Assert.True(10 == a);
            Assert.True(5 != a);
        }
        
        [Fact]
        public void Comparison_DecimalWithNegativeValues_ShouldWork()
        {
            var a = (-5m).As("a");
            
            Assert.True(a > -10m);
            Assert.True(a < 0m);
            Assert.True(a == -5m);
            Assert.True(a != -10m);
        }
    }
}