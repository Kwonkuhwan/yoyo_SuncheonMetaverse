using Suncheon.Admin;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdminRecommBookInfo : MonoBehaviour
{
    private int book_seq;
    private int report_seq;

    [SerializeField] private TMP_Text text_Writer;
    [SerializeField] private TMP_Text text_Complainant;
    [SerializeField] private TMP_Text text_BookName;
    [SerializeField] private Button btn_BookName;

    UI_RecommBookInfo ui_RecommBookInfo;

    public void Init(int _book_seq, int _report_seq, string _writer, string _complainant, string _bookName, UI_RecommBookInfo _ui_RecommBookInfo)
    {
        book_seq = _book_seq;
        report_seq = _report_seq;
        text_Writer.text = _writer;
        text_Complainant.text = _complainant;
        text_BookName.text = _bookName;

        ui_RecommBookInfo = _ui_RecommBookInfo;
        btn_BookName.onClick.AddListener(() => { ui_RecommBookInfo.gameObject.SetActive(true); ui_RecommBookInfo.Init(book_seq, report_seq); });        
    }
}
