using System.Linq;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class Achievement
{
	public string Name;
	public string Description;
	public Texture2D IconIncomplete;
	public Texture2D IconComplete;
	public int RewardPoints;
	public float TargetProgress;
	public bool Secret;

	[HideInInspector]
	public bool Earned = false;
	private float currentProgress = 0.0f;

	public bool AddProgress(float progress)
	{
		if (Earned)
		{
			return false;
		}

		currentProgress += progress;
		if (currentProgress >= TargetProgress)
		{
			Earned = true;
			return true;
		}

		return false;
	}

	public bool SetProgress(float progress)
	{
		if (Earned)
		{
			return false;
		}

		currentProgress = progress;
		if (progress >= TargetProgress)
		{
			Earned = true;
			return true;
		}

		return false;
	}
}

public class AchievementManager : MonoBehaviour
{
	public Achievement[] Achievements;
	public AudioClip EarnedSound;

	private int currentRewardPoints = 0;
	private int potentialRewardPoints = 0;

	void Start()
	{
		ValidateAchievements();
		UpdateRewardPointTotals();
	}

	// Make sure the setup assumptions we have are met.
	private void ValidateAchievements()
	{
		ArrayList usedNames = new ArrayList();
		foreach (Achievement achievement in Achievements)
		{
			if (achievement.RewardPoints < 0)
			{
				Debug.LogError("AchievementManager::ValidateAchievements() - Achievement with negative RewardPoints! " + achievement.Name + " gives " + achievement.RewardPoints + " points!");
			}

			if (usedNames.Contains(achievement.Name))
			{
				Debug.LogError("AchievementManager::ValidateAchievements() - Duplicate achievement names! " + achievement.Name + " found more than once!");
			}
			usedNames.Add(achievement.Name);
		}
	}

	private Achievement GetAchievementByName(string achievementName)
	{
		return Achievements.FirstOrDefault(achievement => achievement.Name == achievementName);
	}

	private void UpdateRewardPointTotals()
	{
		currentRewardPoints = 0;
		potentialRewardPoints = 0;

		foreach (Achievement achievement in Achievements)
		{
			if (achievement.Earned)
			{
				currentRewardPoints += achievement.RewardPoints;
			}

			potentialRewardPoints += achievement.RewardPoints;
		}
	}

	private void AchievementEarned()
	{
		UpdateRewardPointTotals();
		AudioSource.PlayClipAtPoint(EarnedSound, Camera.main.transform.position);
	}

	public void AddProgressToAchievement(string achievementName, float progressAmount)
	{
		Achievement achievement = GetAchievementByName(achievementName);
		if (achievement == null)
		{
			Debug.LogWarning("AchievementManager::AddProgressToAchievement() - Trying to add progress to an achievemnet that doesn't exist: " + achievementName);
			return;
		}

		if (achievement.AddProgress(progressAmount))
		{
			AchievementEarned();
		}
	}

	public void SetProgressToAchievement(string achievementName, float newProgress)
	{
		Achievement achievement = GetAchievementByName(achievementName);
		if (achievement == null)
		{
			Debug.LogWarning("AchievementManager::SetProgressToAchievement() - Trying to add progress to an achievemnet that doesn't exist: " + achievementName);
			return;
		}

		if (achievement.SetProgress(newProgress))
		{
			AchievementEarned();
		}
	}
}
