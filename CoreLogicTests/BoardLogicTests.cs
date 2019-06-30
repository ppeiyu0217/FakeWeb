using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators;
using CoreDAL;
using CoreDAL.Dto;
using CoreLogic;
using CoreLogic.Dto;
using CoreService.Dto;
using CoreWebCommon.Dto;
using ExpectedObjects;
using NSubstitute;
using NUnit.Framework;

namespace CoreLogicTests
{
    [TestFixture]
    public class BoardLogicTests
    {
        private BoardLogicForTest _logicForTest;
        private IBoardDa _boardDa;

        [SetUp]
        public void SetUp()
        {
            _boardDa = Substitute.For<IBoardDa>();
            _logicForTest = new BoardLogicForTest(new Operation(), _boardDa);
        }

        [Test]
        public async Task board_api_has_error()
        {
            GivenBoardQueryResp(false);
            var actual = await WhenGetBoardList();
            ResultShouldBe(actual, false, "Error");
        }

        [Test]
        public async Task get_board_list_success_with_3_real_data_and_2_test_data()
        {
            GivenBoardQueryResp(true);

            GivenBoardDataFromDb(new List<BoardDto>()
            {
                new BoardDto() {Id = "11", IsTest = true},
                new BoardDto() {Id = "12", IsTest = false},
                new BoardDto() {Id = "13", IsTest = true},
                new BoardDto() {Id = "14", IsTest = false},
                new BoardDto() {Id = "16", IsTest = false},
            });

            var actual = await WhenGetBoardList();

            ResultShouldBeSuccess(actual, new List<BoardListItem>
            {
                new BoardListItem() {Id = "12"},
                new BoardListItem() {Id = "14"},
                new BoardListItem() {Id = "16"},
            });
        }

        private static void ResultShouldBeSuccess(IsSuccessResult<BoardListDto> actual, List<BoardListItem> boardListItems)
        {
            var expected = new IsSuccessResult<BoardListDto>()
            {
                IsSuccess = true,
                ReturnObject = new BoardListDto()
                {
                    BoardListItems = boardListItems
                }
            };

            expected.ToExpectedObject().ShouldMatch(actual);
        }

        private void GivenBoardDataFromDb(List<BoardDto> boardDTOs)
        {
            _boardDa.GetBoardData(new[] {"222"}).ReturnsForAnyArgs(
                boardDTOs);

        }


        private static void ResultShouldBe(IsSuccessResult<BoardListDto> actual, bool isSuccess, string errorMessage)
        {
            var expect = new IsSuccessResult<BoardQueryResp>
            {
                IsSuccess = isSuccess,
                ErrorMessage = errorMessage
            };

            expect.ToExpectedObject().ShouldMatch(actual);
        }

        private async Task<IsSuccessResult<BoardListDto>> WhenGetBoardList()
        {
            var actual = await _logicForTest.GetBoardList(new SearchParamDto(), 10);
            return actual;
        }

        private void GivenBoardQueryResp(bool isSuccess)
        {
            _logicForTest.SetBoardQueryResp(new BoardQueryResp()
            {
                IsSuccess = isSuccess,
                Items = new List<BoardQueryRespItem>()
            });
        }




        //    //
        //    //api_issuccess_false
        //    //has_warning_log
        //    //
    }

    internal class BoardLogicForTest : BoardLogic
    {
        private BoardQueryResp _boardQueryResp;
        private List<BoardDto> _boardDto;

        public BoardLogicForTest(Operation operation, IBoardDa da = null) : base(operation, da)
        {
        }

        internal void SetBoardQueryResp(BoardQueryResp boardQueryResp)
        {
            _boardQueryResp = boardQueryResp;
        }

        protected override Task<BoardQueryResp> BoardQueryResp(BoardQueryDto queryDto)
        {
            return Task.FromResult(_boardQueryResp);
        }
    }
}