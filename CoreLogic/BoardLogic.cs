using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDAL;
using CoreDAL.Dto;
using CoreLogic.Dto;
using CoreService;
using CoreService.Dto;
using CoreWebCommon.Dto;
using NLog;

namespace CoreLogic
{
    public class BoardLogic : _BaseLogic
    {
        private readonly IBoardDa _boardDa;
        private ApiService ApiService => new ApiService(GetLogger());

        public BoardLogic(Operation operation, IBoardDa da = null)
            : base(operation)
        {
            _boardDa = da ?? new BoardDa(operation);
        }

        public async Task<IsSuccessResult<BoardListDto>> GetBoardList(SearchParamDto search, int pageSize)
        {
            var queryDto = new BoardQueryDto
            {
                PageSize = pageSize,
                Search = search
            };

            // Http 呼叫 Service 取得資料
            var resp = await BoardQueryResp(queryDto);

            if (!resp.IsSuccess || resp.Items == null)
                return new IsSuccessResult<BoardListDto>() {ErrorMessage = "Error", IsSuccess = false};

            // 使用 http 的資料 從 DB 取得資料
            var settings = _boardDa.GetBoardData(resp.Items.Select(r => r.Id));

            GetLogger().Info(
                string.Join(",", settings.Where(s => s.IsWarning).Select(s => s.Name).ToArray()));

            var boardListDto = new BoardListDto
            {
                BoardListItems = settings
                                 .Where(s => !s.IsTest)
                                 .Select(r => new BoardListItem
                                 {
                                     Id = r.Id,
                                     Name = r.Name
                                 })
            };

            return new IsSuccessResult<BoardListDto>
            {
                IsSuccess = true,
                ReturnObject = boardListDto
            };
        }

        protected virtual async Task<BoardQueryResp> BoardQueryResp(BoardQueryDto queryDto)
        {
            var resp = await ApiService.PostApi<BoardQueryDto, BoardQueryResp>(queryDto);
            return resp;
        }

        private void LogWarning(List<BoardDto> settings, Logger logger)
        {
            logger.Info(
                string.Join(",", settings.Where(s => s.IsWarning).Select(s => s.Name).ToArray()));
        }
    }
}