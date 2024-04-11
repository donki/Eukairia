using EukairiaWeb.Helpers;

namespace EukairiaWeb.Services
{
    public class GlobalService
    {

        public GlobalValues globalValues = new GlobalValues();


        public delegate void RefreshedGlobalValuesDelegate(GlobalValues newValues);
        public RefreshedGlobalValuesDelegate RefreshedGlobalValues;

        public void SetUserName(string userName)
        {
            globalValues.Username = userName;
            if (RefreshedGlobalValues != null)
            {
                RefreshedGlobalValues(globalValues);
            }
        }



    }
}
