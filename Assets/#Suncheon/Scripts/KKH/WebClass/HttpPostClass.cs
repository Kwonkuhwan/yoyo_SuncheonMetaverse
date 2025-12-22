using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GUI;

namespace Suncheon.WebData
{
    #region rtnMsg, rtnCode반환
    public class Response_ReturnMsg
    {
        public string rtnMsg;
        public string rtnCode;
    }
    #endregion

    #region 로그인 관련
    // 로그인 요청
    [Serializable]
    public class Request_Post_Login
    {
        public string userId;
        public string password;

        public Request_Post_Login(string userId, string password)
        {
            Init(userId, password);
        }

        public void Init(string userId, string password)
        {
            this.userId = userId;
            this.password = password;
        }
    }

    // 로그인 응답
    [Serializable]
    public class Response_Post_Login
    {
        public string resultData;
        public string rtnMsg;
        public string rtnCode;
    }

    // 로그인 응답 결과
    [Serializable]
    public class Response_LoginResultData
    {
        public string user_position_code;
        public string user_manage_code;
        public string user_id;
        public string member_class;
        public string name;
        public string nickname;
        public string session_id;
        public string rec_key;
        public string user_no;
    }

    // 닉네임 중복체크 요청
    [Serializable]
    public class Request_NickNameCheck
    {
        public string nickname;

        public Request_NickNameCheck(string nickname)
        {
            Init(nickname);
        }

        public void Init(string nickname)
        {
            this.nickname = nickname;
        }
    }

    // 닉네임 중복체크 응답
    [Serializable]
    public class Response_NickNameCheck
    {
        public string rtnMsg;
        public string rtnCode;
    }
    #endregion

    #region 아바타 관련

    // 아바타 저장
    [Serializable]
    public class Request_AvatarSave
    {
        public string avatar;

        public Request_AvatarSave(string strAvatarInfo)
        {
            Init(strAvatarInfo);
        }

        public void Init(string strAvatarInfo)
        {
            this.avatar = strAvatarInfo;
        }
    }

    // 아바타 저장 응답
    [Serializable]
    public class Response_AvatarSave
    {
        public string rtnMsg;
        public string rtnCode;
    }

    [Serializable]
    public class Request_AvatarLoad
    {
        public string nickname;

        public Request_AvatarLoad(string nickName)
        {
            this.nickname = nickName;
        }
    }

    // 아바타 불러오기
    [Serializable]
    public class Response_AvatarLoad
    {
        public Response_AvatarLoadResultData response_AvatarLoadResultDatas;
        public Response_AvatarLoad(string avatarLoadData)
        {
            response_AvatarLoadResultDatas = JsonUtility.FromJson<Response_AvatarLoadResultData>(avatarLoadData);
        }
    }

    // 아바타 불러오기 결과
    [Serializable]
    public class Response_AvatarLoadResultData
    {
        public string userId;
        public string nickname;
        public string avatar;
    }

    #endregion

    #region 동아리 관련
    // 동아리 생성 요청
    [Serializable]
    public class Request_CreateClubMng
    {
        public string groupNm;
        public string groupSeCode;
        public int enterOption;
        public int groupMovieImg;

        public Request_CreateClubMng(string groupNm, string groupSeCode, int enterOption, int groupMovieImg)
        {
            Init(groupNm, groupSeCode, enterOption, groupMovieImg);
        }

        public void Init(string groupNm, string groupSeCode, int enterOption, int groupMovieImg)
        {
            this.groupNm = groupNm;
            this.groupSeCode = groupSeCode;
            this.enterOption = enterOption;
            this.groupMovieImg = groupMovieImg;
        }
    }

    // 동아리 수정 요청
    [Serializable]
    public class Request_UpdateClubMng
    {
        public string groupSeq;
        public string groupNm;
        public string groupSeCode;
        public int enterOption;
        public int groupMovieImg;

        public Request_UpdateClubMng(string groupSeq, string groupNm, string groupSeCode, int enterOption, int groupMovieImg)
        {
            Init(groupSeq, groupNm, groupSeCode, enterOption, groupMovieImg);
        }

        public void Init(string groupSeq, string groupNm, string groupSeCode, int enterOption, int groupMovieImg)
        {
            this.groupSeq = groupSeq;
            this.groupNm = groupNm;
            this.groupSeCode = groupSeCode;
            this.enterOption = enterOption;
            this.groupMovieImg = groupMovieImg;
        }
    }

    // 동아리 가입 요청
    [Serializable]
    public class Request_JoinClub
    {
        public int groupSeq;

        public Request_JoinClub(int groupSeq)
        {
            Init(groupSeq);
        }

        public void Init(int groupSeq)
        {
            this.groupSeq = groupSeq;
        }
    }

    // 동아리 가입 응답
    [Serializable]
    public class Response_JoinClub
    {
        public string rtnMsg;
        public string rtnCode;
    }

    // 동아리 인원 정보 요청
    [Serializable]
    public class Request_ClubPlayerCnt
    {
        public int groupSeq;

        public Request_ClubPlayerCnt(int groupSeq)
        {
            Init(groupSeq);
        }

        public void Init(int groupSeq)
        {
            this.groupSeq = groupSeq;
        }
    }

    // 동아리 인원 정보 응답
    [Serializable]
    public class Response_ClubPlayerCnt
    {
        public List<Response_ClubPlayerCntResultData> response_ClubPlayerCntResultData;

        public Response_ClubPlayerCnt()
        {
            response_ClubPlayerCntResultData = new List<Response_ClubPlayerCntResultData>();
        }
    }

    // 동아리 인원 정보 결과
    [Serializable]
    public class Response_ClubPlayerCntResultData
    {
        public string registerID;
        public string clubSeq;
        public string rgsde;
        public string updde;
        public string name;
        public string userId;
        public string nickname;
    }
    #endregion

    #region 차단 관련
    // 회원 차단
    [Serializable]
    public class Request_UserBlock
    {
        public string from_id;

        public Request_UserBlock(string blockId)
        {
            this.from_id = blockId;
        }
    }

    // 회원 차단 결과
    [Serializable]
    public class Response_UserBlock
    {
        public string rtnMsg;
        public string rtnCode;
    }

    // 회원 차단 해제
    [Serializable]
    public class Request_UserUnBlock
    {
        public string from_id;
        public Request_UserUnBlock(string blockId)
        {
            this.from_id = blockId;
        }
    }

    // 회원 차단 해제 결과
    [Serializable]
    public class Response_UserUnBlock
    {
        public string rtnMsg;
        public string rtnCode;
    }
    #endregion

    #region 방명록 관련
    [Serializable]
    public class Request_UserCommList
    {
        public string to_id;
        public string order_by;

        public Request_UserCommList(string id, string orderBy)
        {
            to_id = id;
            order_by = orderBy;
        }
    }

    /// <summary>
    /// 받아온 코멘트 리스트
    /// </summary>
    [Serializable]   
    public class Response_CommList
    {
        public List<Response_CommListResultData> response_UserCommListResultDatas = new List<Response_CommListResultData>();
    }

    /// <summary>
    /// 코멘트의 정보
    /// </summary>
    [Serializable]
    public class Response_CommListResultData
    {
        public int commSeq;
        public string commCn;
        public string rgsde;
        public string updde;
        public string toId;
        public string fromId;
        public string fromName;
        public string fromNickname;
        public string report;
    }

    [Serializable]
    public class Request_MyCommList
    {
        public string order_by;

        public Request_MyCommList(string orderBy)
        {
            order_by = orderBy;
        }
    }

    /// <summary>
    /// 코멘트 작성
    /// </summary>
    [Serializable]
    public class Request_WriteComm
    {
        public string to_id;
        public string commCn;

        public Request_WriteComm(string id, string comm)
        {
            to_id = id;
            commCn = comm;
        }
    }

    public class Request_Seq
    {
        public int commSeq;

        public Request_Seq(int Seq)
        {
            commSeq = Seq;
        }
    }
    #endregion

    #region 개인서재 가구
    /// <summary>
    /// 저장할 가구 정보
    /// </summary>
    [Serializable]
    public class Request_SaveFurniture
    {
        public string private_book;

        public Request_SaveFurniture(string PrivateBook)
        {
            private_book = PrivateBook;
        }
    }

    /// <summary>
    /// 가구 저장시 돌아올 메세지
    /// </summary>
    public class Response_SaveFurniture
    {
        public string rtnMsg;
        public string rtnCode;
    }

    public class Room_OwnerId 
    {
        public string user_id;
        public Room_OwnerId(string name) 
        {
            user_id = name;
        }
    }

    #endregion

    #region 미션 관련
    public class Request_MissionUpdate
    {
        public string user_id;
        public int mission1;
        public int mission2;
        public int mission3;
        public int mission4;
        public int mission5;

        public Request_MissionUpdate(string user_id, int mission1, int mission2, int mission3, int mission4, int mission5)
        {
            this.user_id = user_id;
            this.mission1 = mission1;
            this.mission2 = mission2;
            this.mission3 = mission3;
            this.mission4 = mission4;
            this.mission5 = mission5;
        }
    }
    #endregion

    #region 로그인 관련
    public class Resquest_AdminLogin
    {
        public string user_id;
        public string password;

        public Resquest_AdminLogin(string user_id, string password)
        {
            this.user_id = user_id;
            this.password = password;
        }
    }

    public class Response_AdminLogin
    {
        public string resultData;
        public string rtnMsg;
        public string rtnCode;
    }

    public class Response_AdminLoginResult
    {
        public string manager;
        public string user_id;
        public string member_class;
        public string name;
        public string nickname;
        public string session_id;
    }
    #endregion

    #region 공지사항 관련
    [Serializable]
    public class Request_Notice
    {
        public string notice;

        public Request_Notice(string notice)
        {
            this.notice = notice;
        }
    }
    #endregion

    #region 행사 관련
    [Serializable]
    public class Request_EvenetSave
    {
        public string event_name;
        public string event_location;
        public string event_url;
        public string event_start_time;
        public string event_time;

        public Request_EvenetSave(string event_name, string event_location, string event_url, string event_start_time, string event_time)
        {
            this.event_name = event_name;
            this.event_location = event_location;
            this.event_url = event_url;
            this.event_start_time = event_start_time;
            this.event_time = event_time;
        }
    }

    [SerializeField]
    public class Request_EventURLSave
    {
        public int event_seq;
        public string event_url;

        public Request_EventURLSave(int event_seq, string event_url)
        {
            this.event_seq = event_seq;
            this.event_url = event_url;
        }
    }

    [Serializable]
    public class Request_EventDelete
    {
        public int event_seq;
        public Request_EventDelete(int event_seq)
        {
            this.event_seq = event_seq;
        }
    }
    #endregion

    #region 대시보드 관련
    [Serializable]
    public class Request_DashBoardDataCntLoad
    {
        public string date;

        public Request_DashBoardDataCntLoad(string date)
        {
            this.date = date;
        }
    }

    [Serializable]
    public class Response_DashBoardCntResult
    {
        public float avaHour;
        public int newUserCnt;
        public int userCnt;
        public float monthHour;
        public string monthData;
        public int allUserCnt;
        public float weekHour;
        public string weekDate;
    }

    [Serializable]
    public class Request_DashBoardChartLoad
    {
        public string date;

        public Request_DashBoardChartLoad(string date)
        {
            this.date = date;
        }
    }

    [Serializable]
    public class Response_DashBoardChartData
    {
        public List<Response_DashBoardChartResultData> response_DashBoardResultDatas = new List<Response_DashBoardChartResultData>();
        public Response_DashBoardChartData(string jsonData)
        {
            JArray jArray = JArray.Parse(jsonData);
            foreach (var jobj in jArray)
            {
                Response_DashBoardChartResultData result = JsonUtility.FromJson<Response_DashBoardChartResultData>(jobj.ToString());
                response_DashBoardResultDatas.Add(result);
            }
        }
    }

    [Serializable]
    public class Response_DashBoardChartResultData
    {
        public string cnt;
        public string time;
    }
    #endregion

    #region 방명록/책추천 관련
    [Serializable]
    public class Request_GBorRBRestoration
    {
        public int report_seq;
    }

    [Serializable]
    public class Request_GBorRBDelete
    {
        public int report_seq;
    }
    #endregion

    #region 코멘트 읽음 처리
    public class Request_ReadComm
    {
        public int commSeq;

        public Request_ReadComm(int CommSeq)
        {
            commSeq = CommSeq;
        }
    }
    #endregion

    #region 추천도서 작성 관련

    //추천도서 작성내용 저장
    public class Request_SaveRecmComm
    {
        public string title;
        public string content;

        public Request_SaveRecmComm(string Title, string Content)
        {
            title = Title;
            content = Content;
        }
    }
    //삭제할 방명록의 seq넘버 
    public class Request_RecmSeq
    {
        public int recm_seq;

        public Request_RecmSeq(int Recm)
        {
            recm_seq = Recm;
        }
    }

    // 추천 도서 상세 조회
    [Serializable]
    public class Request_SelectRecmBook
    {
        public string recm_seq;
    }

    [Serializable]
    public class Response_SelectRecmBook
    {
        public string recmSeq;
        public string title;
        public string content;
        public string name;
        public string nickname;
        public string rgsde;
        public string updde;
        public string report;
    }
    #endregion

    #region 신고 관련
    /// <summary>
    /// 신고
    /// </summary>
    public class Request_Report 
    {
        public string type;
        public int report_id;

        public Request_Report(string Type, int Seq)
        {
            type = Type;
            report_id = Seq;
        }
    }
    #endregion

    #region 보물찾기 관련
    //추천도서 작성내용 저장
    public class Request_UpsertTreasure
    {
        public int location1;
        public int location2;
        public int location3;
        public int location4;
        public int location5;
        public int location6;
        public int location7;
        public int location8;

        public Request_UpsertTreasure(int location1, int location2, int location3, int location4, int location5, int location6, int location7, int location8)
        {
            this.location1 = location1;
            this.location2 = location2;
            this.location3 = location3;
            this.location4 = location4;
            this.location5 = location5;
            this.location6 = location6;
            this.location7 = location7;
            this.location8 = location8;
        }
    }
    #endregion

    #region 투표 관련
    [Serializable]
    public class Request_UesrVote
    {
        public int vote_seq;
        public int vote_category;

        public Request_UesrVote(int vote_seq, int vote_cateory)
        {
            this.vote_seq = vote_seq;
            this.vote_category = vote_cateory;
        }
    }
    #endregion

    #region 업적 관련
    [Serializable]
    public class Request_OxGameCnt
    {
        public int ox;

        public Request_OxGameCnt(int ox)
        {
            this.ox = ox;
        }
    }

    [Serializable]
    public class Request_CardGameCnt
    {
        public int card;

        public Request_CardGameCnt(int card)
        {
            this.card = card;
        }
    }

    [Serializable]
    public class Request_FeedingGameCnt
    {
        public int feeding;

        public Request_FeedingGameCnt(int feeding)
        {
            this.feeding = feeding;
        }
    }

    [Serializable]
    public class Request_SSLibCntSet
    {
        public int lib1;

        public Request_SSLibCntSet(int cnt)
        {
            this.lib1 = cnt;
        }
    }

    [Serializable]
    public class Request_PBLibCntSet
    {
        public int lib2;

        public Request_PBLibCntSet(int cnt)
        {
            this.lib2 = cnt;
        }
    }

    [Serializable]
    public class Request_YHLibCntSet
    {
        public int lib3;

        public Request_YHLibCntSet(int cnt)
        {
            this.lib3 = cnt;
        }
    }

    [Serializable]
    public class Request_MILibCntSet
    {
        public int lib4;

        public Request_MILibCntSet(int cnt)
        {
            this.lib4 = cnt;
        }
    }

    [Serializable]
    public class Request_SDLibCntSet
    {
        public int lib6;

        public Request_SDLibCntSet(int cnt)
        {
            this.lib6 = cnt;
        }
    }


    [Serializable]
    public class Response_UserMissionInfo
    {
        public string userId;
        public int ox;
        public int card;
        public int feeding;

        public int lib1;
        public int lib2;
        public int lib3;
        public int lib4;
        public int lib5;
        public int lib6;

        public int rcmmBookCnt;
        public int commBoardCnt;

        public int location1;
        public int location2;
        public int location3;
        public int location4;
        public int location5;
        public int location6;
        public int location7;
    }

    [Serializable]
    public class Request_InsertUserReward
    {
        public string reward;
        public Request_InsertUserReward(string reward)
        {
            this.reward = reward;
        }
    }

    [Serializable]
    public class Response_RewardInfo
    {
        public string reward;
    }
    #endregion

}