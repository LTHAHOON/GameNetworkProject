using System.Text;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

    private StringBuilder _sb;
    
    public void SetScoreText(int previousScore = 0, int newScore = 0)
    {
        if(_sb == null)
        {
            _sb = new();
        }
        _sb.Append("Score: ");
        _sb.Append(newScore);
        _scoreText.SetText(_sb);
        _sb.Clear();
    }
    
}
