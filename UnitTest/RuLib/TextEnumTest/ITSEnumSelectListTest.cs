using System.Web.WebPages.Html;

namespace CommonTest.ITSEnumTest
{
    [TestClass()]
    public class ITSEnumSelectListTest
    {
        private SelectListService service;

        [TestInitialize]
        public void Init()
        {
            service = new SelectListService(null, null, null, null, null, null);
        }

        [TestMethod()]
        public void Production_GetITSEnumSelectListItems()
        {
            //arrange
            var Expected = new List<SelectListItem>() {
                new SelectListItem(){ Disabled =false,Selected =false,Group=null,Text="使用者資料匯入批次作業",Value ="901"},
                new SelectListItem(){ Disabled =false,Selected =false,Group = null,Text="部門資料匯入批次作業",Value ="902"},
            };
            var Expected2 = new List<SelectListItem>() {
                new SelectListItem(){ Disabled =false,Selected =true,Text="使用者資料匯入批次作業",Value ="901"},
                new SelectListItem(){ Disabled =false,Selected =false,Text="部門資料匯入批次作業",Value ="902"},
            };

            //act
            var act = service.GetITSEnumSelectListItems(typeof(ProdFakeBatchAppEnum));
            var act2 = service.GetITSEnumSelectListItems(typeof(ProdFakeBatchAppEnum), "901");

            //assert
            ReferenceEquals(Expected, act);
            ReferenceEquals(Expected2, act2);
        }

        [TestMethod()]
        public void HNCB_GetITSEnumSelectListItems()
        {
            //arrange
            var Expected = new List<SelectListItem>() {
                new SelectListItem(){ Disabled =false,Selected =false,Text="台電媒體檔資料匯入",Value ="0201"},
                new SelectListItem(){ Disabled =false,Selected =false,Text="使用者資料匯入批次作業",Value ="901"},
                new SelectListItem(){ Disabled =false,Selected =false,Text="部門資料匯入批次作業",Value ="902"},
            };
            var Expected2 = new List<SelectListItem>() {
                new SelectListItem(){ Disabled =false,Selected =true,Text="台電媒體檔資料匯入",Value ="0201"},
                new SelectListItem(){ Disabled =false,Selected =false,Text="使用者資料匯入批次作業",Value ="901"},
                new SelectListItem(){ Disabled =false,Selected =false,Text="部門資料匯入批次作業",Value ="902"},
            };

            //act
            var act = service.GetITSEnumSelectListItems(typeof(HncbFakeBatchAppEnum));
            var act2 = service.GetITSEnumSelectListItems(typeof(HncbFakeBatchAppEnum), "0201");

            //assert
            ReferenceEquals(Expected, act);
            ReferenceEquals(Expected2, act2);
        }
    }
}