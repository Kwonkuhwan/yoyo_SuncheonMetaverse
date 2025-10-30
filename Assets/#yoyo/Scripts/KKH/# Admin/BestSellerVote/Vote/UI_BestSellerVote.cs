using Newtonsoft.Json.Linq;
using Suncheon.WebData;
using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Suncheon.Admin
{
    public class UI_BestSellerVote : MonoBehaviour
    {
        [SerializeField] private TMP_Text text_VoteName;
        [SerializeField] private TMP_Text text_VoteTotalCnt;

        [SerializeField] private List<TMP_Text> list_BestSellerVoteInfos = new List<TMP_Text>();
        [SerializeField] private TMP_Text text_ProgressType;

        [SerializeField] private Button btn_Reload;

        private void Awake()
        {
            btn_Reload.onClick.AddListener(() => GetVoteResult());
        }

        private void OnEnable()
        {
            GetVoteResult();
        }
                
        private void GetVoteResult()
        {
            UTILS.Log("GetVoteResult");

            StartCoroutine(UTILS.Requset_HttpGetData($"{GameManager.Instance.defaultData.serviceUrl}{GameManager.Instance.defaultData.mngrVoteResult}", (jsonData) =>
            {
                JArray jArray = null;
                try
                {
                    jArray = JArray.Parse(jsonData);
                }
                catch
                {
                    jArray = null;
                }
                if (jArray == null) return;

                Response_VoteResult response_VoteResult = null;
                try
                {
                    if (jArray.Count <= 0) return;
                    response_VoteResult = JsonUtility.FromJson<Response_VoteResult>(jArray.First.ToString());
                }
                catch
                {
                    response_VoteResult = null;
                }

                if (response_VoteResult == null) return;

                SetVoteInfo(response_VoteResult);
            }));
        }

        public class VoteInfo
        {
            public string voteName;
            public int voteCnt;
            public double votePercentage;

            public VoteInfo(string voteName, int voteCnt, double votePercentage)
            {
                this.voteName = voteName;
                this.voteCnt = voteCnt;
                this.votePercentage = votePercentage;
            }
        }

        private void SetVoteInfo(Response_VoteResult result)
        {
            text_VoteName.text = result.title;
            text_VoteTotalCnt.text = $"총 투표 수 : {result.totalVote}";

            List<VoteInfo> votes = new List<VoteInfo>();

            double percentage = 0;
            double data = GetPercentage(double.Parse(result.vote1), double.Parse(result.totalVote), 2);
            if (data == double.NaN) percentage = 0;
            VoteInfo voteInfo = new VoteInfo(result.category1, int.Parse(result.vote1), percentage);
            votes.Add(voteInfo);

            data = GetPercentage(double.Parse(result.vote2), double.Parse(result.totalVote), 2);
            if (!data.Equals(double.NaN)) percentage = data;
            voteInfo = new VoteInfo(result.category2, int.Parse(result.vote2), percentage);
            votes.Add(voteInfo);

            data = GetPercentage(double.Parse(result.vote3), double.Parse(result.totalVote), 2);
            if (!data.Equals(double.NaN)) percentage = data;
            voteInfo = new VoteInfo(result.category3, int.Parse(result.vote3), percentage);
            votes.Add(voteInfo);

            data = GetPercentage(double.Parse(result.vote4), double.Parse(result.totalVote), 2);
            if (!data.Equals(double.NaN)) percentage = data;
            voteInfo = new VoteInfo(result.category4, int.Parse(result.vote4), percentage);
            votes.Add(voteInfo);

            votes.Sort((x, y) => y.votePercentage.CompareTo(x.votePercentage));

            SetBestSellerVoteInfo(votes);

            DateTime startTime = DateTime.ParseExact(result.voteStartTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(result.voteEndTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            if(startTime <= DateTime.Now && endTime > DateTime.Now)
            {
                SetProgressType("진행중");
            }
            else
            {
                SetProgressType("종료");
            }
        }

        private double GetPercentage(double value, double total, int decimalplaces)
        {
            return System.Math.Round(value * 100 / total, decimalplaces);
        }

        private void SetBestSellerVoteInfo(List<VoteInfo> votes)
        {
            for (int i=0; i<votes.Count; i++)
            {
                list_BestSellerVoteInfos[i].text = $"[{votes[i].voteName}] : {votes[i].voteCnt}표 / {votes[i].votePercentage}% / {i+1}등";
            }
        }

        private void SetProgressType(string info)
        {
            text_ProgressType.text = info;
        }
    }
}
