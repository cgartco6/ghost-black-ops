using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class PremiumAdSystem : MonoBehaviour
{
    [Header("Ad Configuration")]
    public string gameId = "1234567";
    public string[] adUnitIds = {"rewardedVideo", "interstitial"};
    public bool testMode = false;
    
    [Header("Revenue Settings")]
    public int adsBetweenMissions = 3;
    public float minECPM = 50f;
    public float maxECPM = 2200f;
    
    private int adsWatched = 0;
    private float totalRevenue = 0f;
    
    void Start()
    {
        Advertisement.Initialize(gameId, testMode);
        StartCoroutine(ShowAdSequence());
    }
    
    IEnumerator ShowAdSequence()
    {
        while (true)
        {
            yield return new WaitUntil(() => adsWatched < adsBetweenMissions);
            
            // Show 3 high-value ads in sequence
            for (int i = 0; i < 3; i++)
            {
                if (Advertisement.IsReady("rewardedVideo"))
                {
                    var options = new ShowOptions { resultCallback = HandleAdResult };
                    Advertisement.Show("rewardedVideo", options);
                    yield return new WaitForSeconds(1f);
                }
                yield return new WaitForSeconds(0.5f);
            }
            
            adsWatched = 0;
        }
    }
    
    void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                float revenue = Random.Range(minECPM, maxECPM) / 1000f; // eCPM to actual revenue
                totalRevenue += revenue;
                adsWatched++;
                
                // Reward player
                GrantAdRewards();
                break;
        }
    }
    
    void GrantAdRewards()
    {
        // Grant premium tokens
        PlayerInventory.Instance.AddTokens(25);
        
        // Grant special items
        if (Random.Range(0f, 1f) > 0.7f)
        {
            PlayerInventory.Instance.AddPremiumItem();
        }
    }
}
