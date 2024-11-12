using RuLib.ValueEnum;
using System.Web.WebPages.Html;

namespace UnitTest.RuLib.ValueEnumTest
{
    [TestClass()]
    public class ValueEnumSelectListTest
    {
        private ValueEnumSelectList service;

        [TestInitialize]
        public void Init()
        {
            service = new ValueEnumSelectList();
        }

        [TestMethod()]
        public void Production_GetValueEnumSelectListItems()
        {
            //arrange
            var Expected = new List<SelectListItem>() {
                new SelectListItem(){ Selected =false,Text="使用者資料匯入批次作業",Value ="901"},
                new SelectListItem(){Selected =false,Text="部門資料匯入批次作業",Value ="902"},
            };
            var Expected2 = new List<SelectListItem>() {
                new SelectListItem(){ Selected =true,Text="使用者資料匯入批次作業",Value ="901"},
                new SelectListItem(){ Selected =false,Text="部門資料匯入批次作業",Value ="902"},
            };

            //act
            var act = service.GetValueEnumSelectListItems(typeof(ProdFakeBatchAppEnum));
            var act2 = service.GetValueEnumSelectListItems(typeof(ProdFakeBatchAppEnum), "901");

            //assert
            ReferenceEquals(Expected, act);
            ReferenceEquals(Expected2, act2);
        }

        [TestMethod()]
        public void HNCB_GetValueEnumSelectListItems()
        {
            //arrange
            var Expected = new List<SelectListItem>() {
                new SelectListItem(){ Selected =false,Text="台電媒體檔資料匯入",Value ="0201"},
                new SelectListItem(){ Selected =false,Text="使用者資料匯入批次作業",Value ="901"},
                new SelectListItem(){ Selected =false,Text="部門資料匯入批次作業",Value ="902"},
            };
            var Expected2 = new List<SelectListItem>() {
                new SelectListItem(){ Selected =true,Text="台電媒體檔資料匯入",Value ="0201"},
                new SelectListItem(){ Selected =false,Text="使用者資料匯入批次作業",Value ="901"},
                new SelectListItem(){ Selected =false,Text="部門資料匯入批次作業",Value ="902"},
            };

            //act
            var act = service.GetValueEnumSelectListItems(typeof(HncbFakeBatchAppEnum));
            var act2 = service.GetValueEnumSelectListItems(typeof(HncbFakeBatchAppEnum), "0201");

            //assert
            ReferenceEquals(Expected, act);
            ReferenceEquals(Expected2, act2);
        }
    }
}