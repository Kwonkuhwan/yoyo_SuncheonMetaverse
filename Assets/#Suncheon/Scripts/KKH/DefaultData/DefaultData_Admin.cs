using UnityEngine;

namespace Suncheon
{
    public partial class DefaultData
    {
        [Space(10)]
        [Header("관리자 도구 관련")]
        #region 관리자 도구 관련
        [Header("관리자 로그인")]
        public string mngrLogin;

        [Header("공지")]
        public string mngrNotice;

        [Header("대쉬 보드")]
        public string mngrDashBoardCntData;
        public string mngrDashBoardChartData;

        [Header("행사")]
        public string mngrEventList;
        public string mngrEventUrlLoad;
        public string mngrEventUrlSave;
        public string mngrEventUrlDelete;
        public string mngrEventSet;

        [Header("미니게임")]
        public string mngrMiniGameSave;
        public string mngrMiniGameLoad;
        public string mngrMiniGameUpdate;

        [Header("NPC")]
        public string mngrNpcFAQLoad;
        public string mngrNpcFAQSave;

        [Header("뮤지엄")]
        public string mngrMuseumImageSave;
        public string mngrMuseumImageLoad;

        [Header("전시공간")]
        public string mngrExhibitionLoad;
        public string mngrExhibitionSave;
        public string mngrExhibitionUpdate;

        [Header("스크린")]
        public string mngrScreenLoad;
        public string mngrScreenSave;
        public string mngrScreenUpdate;

        [Header("배너")]
        public string mngrBannerLoad;
        public string mngrBannerSave;
        public string mngrBannerUpdate;

        [Header("방명록/책 추천 관리")]
        public string mngrGBookOrRBookLoad; // 방명록/책추천 조회
        public string mngrGBookOrRBookRestoration;  // 방명록/책추천 복구
        public string mngrGBookOrRBookDelete;       // 방명록/책추천 삭제

        [Header("어린이 그림 전시")]
        public string mngrChildrenArtSave;
        public string mngrChildrenArtLoad;

        [Header("투표")]
        public string mngrVoteProcess;      // 투표 진행 유무
        public string mngrVoteInsert;       // 투표 설정
        public string mngrVoteResult;       // 투표 결과 조회
        public string mngrVoteSelectUser;   // 사용자 투표 유무
        public string mngrVoteSelect;       // 사용자 투표 입력

        [Header("채널 초기화")]
        public string devChannelReset;
        #endregion
    }
}