using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

/////////////////////////////////////////////////////////Wayline Jeffries//////////////////////////////////////////////////////////////////////

namespace PackageChallenge
{
    public class Packer
    {
        public static string Pack(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            StringBuilder result = new StringBuilder();

            foreach (string line in lines)
            {
                /// Taking one line and splitting it by the colon so that the weight Limit is stored and the items separately.
                string[] parts = line.Split(':');
                decimal weightLimit = decimal.Parse(parts[0]);
                string itemsString = parts[1].Trim();

                /// Creating a List to store all the items 
                /// Creating an Array and storing each item separately by splitting it with the spaces between each item.
                List<Item> items = new List<Item>();
                string[] itemStrings = itemsString.Split(' ');

                /// The foreach loop will loop through the itemString Array and read one item at a time
                /// So far the each item looks like -> (1,53.38,€45)
                foreach (string itemString in itemStrings)
                {
                    if (itemString.StartsWith("(") && itemString.EndsWith(")"))
                    {
                        /// The item "(1,53.38,€45)" is then split into parts, separating the index, weight and cost
                        /// After the split the items are stored and converted to their own assigned variables
                        string[] partsArray = itemString.Trim('(', ')').Split(',');

                        int index = int.Parse(partsArray[0]);
                        decimal cost = decimal.Parse(partsArray[2].Replace("€", ""));
                        decimal weight = decimal.Parse(partsArray[1] , CultureInfo.InvariantCulture);

                        /// A new Item object is created and initialized with the provided values for index, weight, and cost.
                        /// Item is a custom class defined in the code(line 155), representing an item with an index, weight, and cost.
                        Item item = new Item(index, weight, cost);
                        items.Add(item);
                    }
                    else
                    {
                        /// Else if the itemString are not in brackets
                        /// The item "1,53.38,€45" is then split into parts, separating the index, weight and cost 
                        /// After the split the items are stored and converted to their own assigned variables
                        string[] partsArray = itemString.Split(',');

                        int index = int.Parse(partsArray[0]);
                        decimal weight = decimal.Parse(partsArray[1]);
                        decimal cost = decimal.Parse(partsArray[2].Replace("€", ""));

                        /// A new Item object is created and initialized with the provided values for index, weight, and cost.
                        /// Item is a custom class defined in the code(line 155), representing an item with an index, weight, and cost.
                        Item item = new Item(index, weight, cost);
                        items.Add(item);
                    }
                }/// End of foreach

                /// The items are choosen based on the given weight limit.
                /// The ChooseItems method is called with two arguments: items and weightLimit.
                /// The method returns a list of Item objects representing the chosen items that satisfy the weight limit constraint and have the maximum total cost.
                List<Item> chosenItems = ChooseItems(items, weightLimit);


                /// Checking if there is any elements in the List
                if (chosenItems.Count > 0)
                {
                    /// Append the indexes of chosen items to the result string
                    result.AppendLine(string.Join(",", chosenItems.Select(item => item.Index)));
                }
                else
                {
                    /// If no items can be chosen, append a dash to the result string
                    result.AppendLine("-");
                }
            }

            return result.ToString();
        }

        /// ChooseItems is a method that takes in a List of items and the weightLimit
        /// This method will determine whether the items entered meets the constraints and the weightLimit and will return a List of those items.
        private static List<Item> ChooseItems(List<Item> items, decimal weightLimit)
        {
            int itemCount = items.Count;

            /// A 2D array is created to store the maximum cost for each item and weight limit combination
            decimal[,] dp = new decimal[itemCount + 1, (int)(weightLimit * 100) + 1];

            /// Calculate the maximum cost for each item and weight limit combination
            for (int i = 1; i <= itemCount; i++)
            {
                Item item = items[i - 1];

                for (int w = 0; w <= weightLimit * 100; w++)
                {
                    /// Checking if the current item can be included in the package
                    if (item.Weight * 100 <= w)
                    {
                        decimal costWithItem = item.Cost + dp[i - 1, w - (int)(item.Weight * 100)];
                        decimal costWithoutItem = dp[i - 1, w];

                        /// Choose the maximum cost between including and excluding the current item
                        dp[i, w] = Math.Max(costWithItem, costWithoutItem);
                    }
                    else
                    {
                        /// Cannot include the current item, so the cost remains the same as excluding the item
                        dp[i, w] = dp[i - 1, w];
                    }
                }
            }

            /// Retrieving the chosen items based on the maximum cost calculated
            /// A new List is created to store the chosen Items
            /// Setting row to the last item to start from the last item to the first
            List<Item> chosenItems = new List<Item>();
            int row = itemCount;
            int col = (int)(weightLimit * 100);

            /// Loop until the first row or column is reached
            while (row > 0 && col > 0)
            {
                /// Check if the current cell's value differs from the cell above it
                if (dp[row, col] != dp[row - 1, col])
                {
                    /// Retrieve the item corresponding to the current row
                    /// Add the chosen item to the list
                    Item chosenItem = items[row - 1];
                    chosenItems.Add(chosenItem);

                    /// Reduce the column index by the weight of the chosen item (in scaled form)
                    col -= (int)(chosenItem.Weight * 100);
                }

                row--;
            }

            /// Reversing the list of chosen items since we retrieved them in reverse order
            chosenItems.Reverse();

            return chosenItems;
        }


    }

    public class Item
    {
        public int Index { get; set; }
        public decimal Weight { get; set; }
        public decimal Cost { get; set; }

        public Item(int index, decimal weight, decimal cost)
        {
            Index = index;
            Weight = weight;
            Cost = cost;
        }
    }

    public class APIException : Exception
    {
        public APIException(string message) : base(message)
        {
        }
    }
}
