#if UNITY_EDITOR
using System;
using System.Collections;
using NUnit.Framework;
using Platformer.Core;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class PlayerLandedTests
{
    private GameObject ground;
    private PlayerController player;
    private Action<PlayerLanded> onLanded;

    [UnityTest]
    public IEnumerator PlayMode_PlayerLanding_SchedulesPlayerLanded()
    {
        Simulation.Clear();
        ground = new GameObject("TestGround");
        ground.transform.position = new Vector3(1000, 0, 0);
        ground.AddComponent<BoxCollider2D>().size = new Vector2(10, 1);

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        Assert.That(prefab, Is.Not.Null, "Не найден префаб Player.");
        var instance = Object.Instantiate(prefab, new Vector3(1000, 4, 0), Quaternion.identity);
        player = instance.GetComponent<PlayerController>();
        Assert.That(player, Is.Not.Null);
        instance.layer = 0;
        player.controlEnabled = false;
        player.jumpState = PlayerController.JumpState.InFlight;
        Physics2D.SyncTransforms();

        int landingCount = 0;
        bool groundedAtEvent = false;
        onLanded = ev =>
        {
            if (ev.player != player) return;
            landingCount++;
            groundedAtEvent = player.IsGrounded;
        };
        PlayerLanded.OnExecute += onLanded;

        yield return null;
        Simulation.Tick();
        Assert.That(player.IsGrounded, Is.False, "Игрок должен начать в воздухе.");
        Assert.That(landingCount, Is.Zero, "До приземления события быть не должно.");

        float deadline = Time.realtimeSinceStartup + 5f;
        while (landingCount == 0 && Time.realtimeSinceStartup < deadline)
        {
            yield return null;
            Simulation.Tick();
        }

        Assert.That(landingCount, Is.EqualTo(1), "Приземление должно запланировать PlayerLanded.");
        Assert.That(groundedAtEvent, Is.True, "Событие должно относиться к контакту с землей.");

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForFixedUpdate();
            yield return null;
            Simulation.Tick();
        }
        Assert.That(landingCount, Is.EqualTo(1), "Стояние на платформе не должно повторять событие.");
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (onLanded != null) PlayerLanded.OnExecute -= onLanded;
        Simulation.Clear();
        if (player != null) Object.Destroy(player.gameObject);
        if (ground != null) Object.Destroy(ground);
        yield return null;
    }
}
#endif
