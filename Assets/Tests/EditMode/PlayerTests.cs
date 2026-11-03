using NUnit.Framework;
using Platformer.Mechanics;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PlayerTests
{
    [Test]
    public void EditMode_PlayerTag_IsCorrect()
    {
        const string scenePath = "Assets/Scenes/SampleScene.unity";
        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(scenePath);
        bool openedByTest = !scene.isLoaded;
        if (openedByTest)
            scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

        try
        {
            Assert.That(scene.IsValid() && scene.isLoaded, Is.True);
            PlayerController player = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                player = root.GetComponentInChildren<PlayerController>(true);
                if (player != null) break;
            }

            Assert.That(player, Is.Not.Null, "На SampleScene отсутствует игрок.");
            Assert.That(player.CompareTag("Player"), Is.True);
            Assert.That(player.GetComponent<Health>(), Is.Not.Null);
        }
        finally
        {
            if (openedByTest) EditorSceneManager.CloseScene(scene, true);
        }
    }
}
