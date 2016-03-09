using System;
using System.Data.Linq;

namespace MoreLINQToSQL
{
    public partial class Order
    {
        #region OnValidate
        /// <summary>
        /// OnValidate
        /// </summary>
        /// <param name="action"></param>
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            switch (action)
            {
                case ChangeAction.Insert:
                case ChangeAction.Update:
                    //Validate order Date
                    if (this.OrderDate < System.DateTime.Today)
                    {
                        throw new Exception("Order date can't be before today");
                    }

                    //Validate the order and required date
                    if (this.RequiredDate < this.OrderDate)
                    {
                        throw new Exception("Required date can't be before order date");
                    }
                    break;

                case ChangeAction.Delete:
                    break;
                default:
                    break;

            }
        }
        #endregion

        #region OrderDate

        /// <summary>
        /// Validate when order date is being changed, do not allow it
        /// </summary>
        /// <param name="value"></param>
        partial void OnOrderDateChanging(System.Nullable<System.DateTime> value)
        {
            if (this.OrderDate.HasValue && value != this.OrderDate )
            {
                throw new Exception("Can't change the order date of an existing order");
            }
        }

        #endregion
    }
}