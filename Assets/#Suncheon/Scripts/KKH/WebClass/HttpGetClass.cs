using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suncheon.WebData
{
    #region 로그인 관련
    // 비회원 로그인 응답
    [Serializable]
    public class Response_NonMemberLogin
    {
        public string resultData;
        public string rtnMsg;
        public string rtnCode;
    }

    // 비회원 로그인 결과
    [Serializable]
    public class Response_NonMemberLoginResultData
    {
        public string user_id;
        public string member_class;
        public string name;
        public string session_id;
        public string nickname;
    }
    #endregion

    #region 채널 관련
    // 채널 모든 정보 응답
    [Serializable]
    public class Response_ChannelUserCnt
    {
        public List<Response_ChannelUserCntResultData> channleUserCnt;
        public string allUserCnt;
        public string channelCnt;
    }

    // 채널 정보 결과
    [Serializable]
    public class Response_ChannelUserCntResultData
    {
        public string channel;
        public string cnt;
    }

    // 해당 채널 증가 응답
    [Serializable]
    public class Response_ChannelUserCntPlus
    {
        public int channel;
        public string rtnCode;
    }

    // 해당 채널 감소 응답
    [Serializable]
    public class Response_ChannelUserCntMinus
    {
        public int channel;
        public string rtnCode;
    }
    #endregion

    #region 동아리 관련    

    // 내가 만든 동아리 조회
    [Serializable]
    public class Response_CreatedGroupList
    {
        public List<Response_CreatedGroupListResultData> response_CreatedGroupListResultDatas;
    }

    [Serializable]
    public class Response_CreatedGroupListResultData
    {
        public int groupSeq;
        public string clubName;
        public string clubTemplateCode;
        public int userLimit;
        public string clubType;
        public string viewCount;
        public string deleteYn;
        public string createrEmail;
        public string rgsde;
        public string updde;
    }

    // 내가 가입한 동아리 조회
    [Serializable]
    public class Response_JoinedGroupList
    {
        public List<Response_JoinedGroupListResultData> response_JoinedGroupListResultDatas;
    }

    // 가입한 동아리 결과
    [Serializable]
    public class Response_JoinedGroupListResultData
    {
        public int groupSeq;
        public string clubName;
        public string clubType;
        public string clubTemplateCode;
        public int viewCount;
        public string deleteYn;
        public string createrEmail;
        public string rgsde;
        public string updde;
        public string clubInviteCode;
        public int userLimit;
        public string nickname;
        public string userId;
        public string name;
    }
    #endregion

    #region 차단 관련
    // 차단 목록 조회
    [Serializable]
    public class Response_MyBlockList
    {
        public List<Response_MyBlockListResultData> response_MyBlockListResults = new List<Response_MyBlockListResultData>();
    }

    // 차단 목록 조회 결과
    [Serializable]
    public class Response_MyBlockListResultData
    {
        public string blockId;
        public string nickname;
        public string name;
        public string insertTime;
    }
    #endregion

    #region 개인서재 가구
    //유저 및 가구정보
    [Serializable]
    public class Get_RoomData
    {
        public string userId;
        public string privateBook;
    }

    /// <summary>
    /// privateBook 내용물. 벽지색상, 각 가구의 데이터
    /// </summary>
    [Serializable]
    public class Room_InteriorData
    {
        public List<Furniture_TransformData> response_FurnitureDatas; //가구의 정보가 담긴 리스트

        //맵 타일의 색상
        public string wallColor_R;
        public string wallColor_G;
        public string wallColor_B;
        public string floorColor_R;
        public string floorColor_G;
        public string floorColor_B;

        public Room_InteriorData()
        {
            response_FurnitureDatas = new List<Furniture_TransformData>();
        }
    }

    /// <summary>
    /// 각 가구의 트랜스폼 값
    /// </summary>
    [Serializable]
    public class Furniture_TransformData
    {
        public string posX;
        public string posY;
        public string posZ;
        public string rotY;
        public string FurnitureIndex;
    }
    #endregion

    #region 닉네임으로 아이디 조회
    public class Request_UserID
    {
        public string nickname;

        public Request_UserID(string Nickname)
        {
            nickname = Nickname;
        }
    }

    public class Response_UserID
    {
        public string userId;
    }
    #endregion

    #region 미션 정보 관련
    [Serializable]
    public class Response_MissionInfo
    {
        public int mission1 = 0;
        public int mission2 = 0;
        public int mission3 = 0;
        public int mission4 = 0;
        public int mission5 = 0;
        public string inserttime;
        public string updatetime;
    }
    #endregion

    #region 업적 관련
    [Serializable]
    public class Response_AchievementInfo
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
    #endregion

    #region 관리자 도구 관련
    #region 행사 관련
    public class Response_EventList
    {
        public List<Response_EventListData> response_EventListDatas = new List<Response_EventListData>();

        public Response_EventList(string jsonData)
        {
            JArray jArray = JArray.Parse(jsonData);
            foreach (var obj in jArray)
            {
                Response_EventListData result = JsonUtility.FromJson<Response_EventListData>(obj.ToString());
                response_EventListDatas.Add(result);
            }
        }
    }

    public class Response_EventListData
    {
        public int eventSeq;
        public string eventName;
        public string eventLocation;
        public string eventStartTime;
        public int eventTime;
        public string eventUrl;
    }
    #endregion

    #region 미니게임 관련
    //{"gameSeq":1,"csv":"game.csv","creatorId":"suncheonMng1","rgsde":1702474513079,"updde":1702474513079,"csvLength":1000}
    public class Response_MiniGameLoad
    {
        public string gameSeq;
        public string csv;
        public string creatorId;
        public string updatorId;
        public string rgsde;
        public string updde;
        public int csvLength;
    }
    #endregion

    #region 전시 관련
    public class Response_ExhibitionLoad
    {
        public string exhibition_url;
        public string exhibition_img;
        public string rgsde;
        public string updde;
        public string creator_id;
        public string updator_id;
        public int img_length;
    }
    #endregion

    #region 스크린 관련
    public class Response_ScreenLoad
    {
        public string screen_url;
        public string screen_img;
        public string rgsde;
        public string updde;
        public string creator_id;
        public string updator_id;
        public int img_length;
    }
    #endregion

    #region 배너 관련
    [Serializable]
    public class Response_BannerLoad
    {
        public string banner_url;
        public string banner_img;
        public string rgsde;
        public string updde;
        public string creator_id;
        public string updator_id;
        public int img_length;
    }
    #endregion

    #region NPC(FAQ)
    public class Response_NPCLoad
    {
        public string faq_seq;
        public string faq_file;
        public string rgsde;
        public string creator_id;
        public string faq_length;
        public string updde;
    }

    #endregion

    #region 방명록/책추천 관리
    public class Response_GBorRBLoad
    {
        public string reportSeq;
        public string type;
        public string reportId;
        public string reporterName;
        public string writerName;
        public string reportComm;
    }
    #endregion

    #region 어린이 그림 전시
    [Serializable]
    public class Response_ChildrenEaselLoad
    {
        public string artNumber;
        public string artTitle;
        public string fileName;
        public string fileLength;
        public string rgsde;
        public string updde;
    }
    #endregion

    #region 해지면 열리는 미술관
    public class Response_MuseumLoad
    {
        public int museumSeq;
        public string img;
        public int imgLength;
        public string rgsde;
        public string updde;
    }
    #endregion

    #endregion

    #region 추천도서 관련

    //나의 추천도서 조회
    public class Request_MyRecm
    {
        public string myRecm;

        public Request_MyRecm(string MyRecm)
        {
            myRecm = MyRecm;
        }
    }

    //추천코멘트 전체 리스트
    public class Response_RecmCommList
    {
        public List<Response_RecmDatas> RecmCommListData = new List<Response_RecmDatas>();
    }

    //추천코멘트 내부 정보
    public class Response_RecmDatas
    {
        public string recmSeq;
        public string title;
        public string content;
        public string recommendId;
        public string name;
        public string nickname;
        public string rgsde;
        public string updde;
        public string report;
    }
    #endregion    

    #region 보물찾기 관련
    [Serializable]
    public class Response_Treasure
    {
        public int location1 = 0;
        public int location2 = 0;
        public int location3 = 0;
        public int location4 = 0;
        public int location5 = 0;
        public int location6 = 0;
        public int location7 = 0;
        public int location8 = 0;
        public string inserttime;
        public string updatetime;
    }

    #endregion

    #region 투표 관련
    public class Response_VoteResult
    {
        public string voteSeq;
        public string title;
        public string category1;
        public string category2;
        public string category3;
        public string category4;
        public string totalVote;
        public string vote1;
        public string vote2;
        public string vote3;
        public string vote4;
        public string voteStartTime;
        public string voteEndTime;        
    }

    public class Response_VoteSelect
    {
        public string voteSeq;
        public string title;
        public string voteStartTime;
        public string voteEndTime;

        public string category1;
        public string category2;
        public string category3;
        public string category4;

        public string categoryFile1;
        public string categoryFile2;
        public string categoryFile3;
        public string categoryFile4;

        public string category1Length;
        public string category2Length;
        public string category3Length;
        public string category4Length;

        public string rgsde;

        public string msg;
    }
    #endregion

    #region 삼산 한점 미술관
    public class Response_SS_Museum
    {
        public string n;
        public int nLength;
        public string e;
        public int eLength;
        public string w;
        public int wLength;
        public string s;
        public int sLength;
        public string creatorId;
        public int updde;
    }
    #endregion
}