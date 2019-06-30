using System.Collections.Generic;
using System.Linq;
using CoreDAL;
using CoreDAL.Dto;
using CoreService.Dto;

namespace CoreLogic
{
    public class QueryBoardFromDb
    {
        private readonly BoardDa _boardDa;

        public QueryBoardFromDb(BoardDa boardDa)
        {
            _boardDa = boardDa;
        }

        public List<BoardDto> GetSettingFromDb(BoardQueryResp resp)
        {
            return _boardDa.GetBoardData(resp.Items.Select(r => r.Id));
        }
    }
}