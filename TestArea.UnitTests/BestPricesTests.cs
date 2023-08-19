using NUnit.Framework;

namespace TestArea.UnitTests;

public class BestPricesTests
{
    [Test]
    public void Executing()
    {
        // int[] prices = { 2, 4, 1 };
        int[] prices = { 7,1,5,3,6,4 };
        // int[] prices = { 2,1,2,1,0,1,2 };
        // int[] prices = { 7,6,4,3,1 };

        int minKey = 0;
        int maxKey = 0;

        for (int i = 0; i < prices.Length; i++)
        {
            for (int j = 0; j < maxKey; j++)
            {
                if (prices[minKey] > prices[j])
                {
                    minKey = j;
                }
            }

            if (minKey > maxKey || prices[maxKey] < prices[i])
            {
                maxKey = i;
            }
        }

        int result = 0;
        if (maxKey == minKey)
        {
        }
        else
        {
            result = prices[maxKey] - prices[minKey];
        }

        Assert.Equals(0, result);
    }
}