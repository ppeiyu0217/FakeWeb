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
        [Test]
        public async Task board_api_has_error()
        {
            var logic = new BoardLogicForTest(new Operation());

            logic.SetBoardQueryResp(new BoardQueryResp()
            {
                IsSuccess = false,
            });

            var actual = await logic.GetBoardList(new SearchParamDto(), 10);

            var expect = new IsSuccessResult<BoardQueryResp>
            {
                IsSuccess = false,
                ErrorMessage = "Error"
            };

            expect.ToExpectedObject().ShouldMatch(actual);
        }


        [Test]
        public void if_success_pass_isTest_return_true()
        {
            var boardDto = new List<BoardDto>()
            {
                new BoardDto() {Id = "1", Name = "PY", IsTest = true},
                new BoardDto() {Id = "2", Name = "PYY", IsTest = false},
            };


            var expectBoardList = new BoardListDto()
            {
                BoardListItems = new BoardListItem[]
                {
                    new BoardListItem() {Id = "2", Name = "PYY"}
                }
            };

            var logic = new BoardLogic(new Operation());
            var act = logic.GetBoardList(new SearchParamDto(), 1);

        }

        //
        //api_issuccess_false
        //has_warning_log
        //
    }

    internal class BoardLogicForTest : BoardLogic
    {
        private BoardQueryResp _boardQueryResp;

        public BoardLogicForTest(Operation operation, BoardDa da = null) : base(operation, da)
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