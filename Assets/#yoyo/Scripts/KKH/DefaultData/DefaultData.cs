using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Suncheon
{
    [Serializable]
    public partial class DefaultData
    {
        // 파일 접근 Url
        public string fileUrl;

        [Space(10)]

        // 서비스 Url
        public string serviceUrl;
        //"http://192.168.1.21:8088/",
        //"https://metalibrary.suncheon.go.kr/suncheonlib/",

        [Space(10)]
        public string openAIKey;

        [Space(10)]

        [Header("로그인 관련")]
        #region 로그인 관련
        // 로그인 Url
        public string loginUrl;
        // 로그아웃
        public string logoutUrl;
        // 비회원 로그인 Url
        public string nonLoginUrl;
        // 중복 닉네임 체크 Url
        public string nickNameCheckUrl;
        // 아바타 저장 Url
        public string avatarSaveUrl;
        // 아바타 불러오기 Url
        public string avatarLoadUrl;
        #endregion

        [Space(10)]

        [Header("채널 관련")]
        #region 채널 관련
        // 전체 채널 카운팅 Url
        public string channalCntUrl;
        // 해당 채널 증가 Url
        public string channalCntPlusUrl;
        // 해당 채널 감소 Url
        public string channalCntMinusUrl;
        #endregion

        [Space(10)]

        [Header("동아리 관련")]
        #region 동아리 관련
        // 동아리 생성 Url
        public string createClubUrl;
        // 동아리 수정 Url
        public string updateClubInfoUrl;
        // 가입 동아리 조회 Url
        public string joinClubUrl;
        // 가입한 회원들 조회 Url
        public string getClubPeopleInfoUrl;
        // 회원 내보내기 Url
        public string kickClubUrl;
        // 동아리 나가기 Url
        public string quitClubUrl;
        // 동아리 삭제 Url
        public string deleteClubUrl;
        // 생성한 동아리 조회 Url(사용 안할듯?)
        public string createdClubInfoUrl;
        // 가입한 동아리 조회 Url
        public string joinedClubInfoUrl;
        // 동아리 가입 가능 여부 조회 Url
        public string enterFlagClubUrl;
        #endregion

        [Space(10)]

        [Header("차단 관련")]
        #region 차단 관련
        // 회원 차단 Url
        public string userBlockUrl;
        // 회원 차단 해제 Url
        public string userUnBlockUrl;
        // 회원 차단 목록 Url
        public string userBlockListUrl;

        // 상대가 날 차단했는지 조회
        public string checkBlockFlag;
        #endregion

        [Space(10)]

        [Header("방명록 관련")]
        #region 방명록 관련
        public string userCommList;
        public string writeComm;
        public string myCommList;
        public string DelComm;
        #endregion

        [Space(10)]

        [Header("가구 관련")]
        #region 가구관련
        public string FurnitureSaveUrl;   //가구 정보 저장
        public string MyFurnitureLoadUrl; //가구 정보 불러오기
        public string userFurnitureLoad;  //다른 유저 가구 정보 가져오기?
        #endregion

        [Header("아이디 조회")]
        public string userIdCheck; //타 유저의 아디 확인

        [Header("스탬프")]
        public string missionInfoLoad;
        public string missionInfoUpdate;

        [Header("방명록")]
        public string newComm;
        public string ReadnewComm;

        [Header("방명록/책 신고")]
        public string report;

        [Header("추천도서")]
        public string SaveRecmComm;
        public string GetRecmList;
        public string SelectRecmBook;
        public string DeleteRecmComm;

        [Header("보물찾기")]
        public string upsertUserTreasure;       // 사용자 보물 획득
        public string userTreasure;             // 보물찾기 조회

        [Header("업적")]
        public string userMiniGameCntSet;
        public string userLibCntPlus;
        public string userMissionInfo;
        public string insertUserReward;
        public string userRewardInfo;

        [Header("한점도서관")]
        public string ssMuseumLoad;
        public string ssMuseumSet;
    }
}
