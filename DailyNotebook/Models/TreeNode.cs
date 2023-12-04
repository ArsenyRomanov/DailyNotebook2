using System.Collections.Generic;
using System.ComponentModel;

namespace DailyNotebook.Models
{
    public class TreeNode
    {
        public Worksheet Data { get; set; }
        public TreeNode? Left { get; set; }
        public TreeNode? Right { get; set; }

        public TreeNode(Worksheet worksheet) { Data = worksheet; }

        public void Insert(TreeNode node)
        {
            if (node.Data.LastOpenedDate < Data.LastOpenedDate)
            {
                if (Left == null) Left = node;
                else Left.Insert(node);
            }
            else
            {
                if (Right == null) Right = node;
                else Right.Insert(node);
            }
        }

        public BindingList<Worksheet> Transform(BindingList<Worksheet>? elements = null)
        {
            elements ??= new BindingList<Worksheet>();

            Right?.Transform(elements);
            elements.Add(Data);
            Left?.Transform(elements);

            return elements;
        }
    }
}
