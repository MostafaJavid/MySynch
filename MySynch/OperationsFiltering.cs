// -----------------------------------------------------------------------
// <copyright file="OperationListFiltering.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MySynch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using System.Windows.Controls;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class OperationsFiltering
    {
        Dictionary<OperationType, bool> conditions;
        public OperationsFiltering(ItemCollection collection)
        {
            this.conditions = new Dictionary<OperationType, bool>();
            foreach (var item in collection)
            {
                var checkbox = item as CheckBox;
                conditions.Add((OperationType)Enum.Parse(typeof(OperationType), checkbox.Content.ToString()), checkbox.IsChecked.Value);
            }
        }

        public IEnumerable<SOperation> Filter(IEnumerable<SOperation> operations)
        {
            var result = operations.Where(a => CheckConditions(a));
            return result;
        }

        private bool CheckConditions(SOperation op)
        {
            if (op == null)
            {
                return true;
            }
            else
            {
                return conditions[op.Operation];
            }
        } 
    }
}
