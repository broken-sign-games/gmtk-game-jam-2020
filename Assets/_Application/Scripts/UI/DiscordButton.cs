using UnityEngine;

namespace GMTK2020.UI
{
    public class DiscordButton : MonoBehaviour
    {
        [SerializeField] private string discordInviteLink = "";

        public void OpenDiscord()
        {
            Application.OpenURL(discordInviteLink);
        }
    }
}
