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
using NUnit.Framework;

namespace CoreLogicTests
{
    [TestFixture]
    public class BoardLogicTests
    {
        private BoardLogicForTest _logicForTest;

        [SetUp]
        public void SetUp()
        {
            _logicForTest = new BoardLogicForTest(new Operation());
        }

        [Test]
        public async Task board_api_has_error()
        {
            GivenBoardQueryResp(false);
            var actual = await WhenGetBoardList();
            ResultShouldBe(actual, false, "Error");
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