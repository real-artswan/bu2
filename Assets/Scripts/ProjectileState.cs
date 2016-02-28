using System;
using UnityEngine;

public class ProjectileState
{
    internal GameState gameState = null;

    internal BaboProjectileType typeID = BaboProjectileType.PROJECTILE_NONE;
    internal byte playerID = 0;
    internal BaboWeapon weaponID = BaboWeapon.WEAPON_NO;
    internal byte nuzzleID = 0;
    internal Int32 uniqueID = -1;
    internal Vector3 position = Vector3.zero;
    internal Vector3 vel = Vector3.zero;
    internal byte stickToPlayer = 255;

    private GameObject projectileObject;

    private float lifeTime = 0;

    public ProjectileState(GameState gameState, Vector3 position, Vector3 velocity, BaboProjectileType typeID, BaboWeapon weaponID) {
        this.gameState = gameState;

        this.position = position;
        this.typeID = typeID;
        this.weaponID = weaponID;

        Vector3 projectileVelocity = velocity;
        Quaternion initRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
        Vector3 initScale = new Vector3(0.1f, 0.1f, 0.1f);
        GameObject prefab = null;
        if (typeID != BaboProjectileType.PROJECTILE_DROPED_WEAPON)
            prefab = gameState.serverVars.weaponsVars.getProjectile(typeID).prefab;
        switch (typeID) {
            case BaboProjectileType.PROJECTILE_ROCKET:
                lifeTime = 10;
                projectileVelocity *= 2.5f;
                initRotation = Quaternion.LookRotation(projectileVelocity);
                break;
            case BaboProjectileType.PROJECTILE_GRENADE:
                lifeTime = 2;
                projectileVelocity *= 5f;
                projectileVelocity.y += 5;
                break;
            case BaboProjectileType.PROJECTILE_COCKTAIL_MOLOTOV:
                lifeTime = 10;
                projectileVelocity *= 6f;
                projectileVelocity.y += 2;
                break;
            case BaboProjectileType.PROJECTILE_FLAME:
                lifeTime = 10;
                initScale = Vector3.one;
                break;
            case BaboProjectileType.PROJECTILE_LIFE_PACK:
                lifeTime = 20;
                break;
            case BaboProjectileType.PROJECTILE_DROPED_GRENADE:
                lifeTime = 25;
                break;
            case BaboProjectileType.PROJECTILE_DROPED_WEAPON:
                lifeTime = 30;
                prefab = gameState.serverVars.weaponsVars.getWeapon(weaponID).prefab;
                initScale = new Vector3(0.4f, 0.4f, 0.4f);
                break;
            default:
                if (Debug.isDebugBuild)
                    Debug.Log("Unexpected projectile: " + typeID.ToString());
                break;
        }
        this.vel = projectileVelocity;
        if (prefab != null) {
            projectileObject = GameObject.Instantiate(prefab, position, initRotation) as GameObject;
            projectileObject.transform.localScale = initScale;
        }
    }

    public void destroy() {
        if (projectileObject == null)
            return;
        GameObject.Destroy(projectileObject);
    }

    public void update() {
        if (projectileObject == null)
            return;
        float delay = Time.deltaTime; //cache var

        lifeTime -= delay;
        if (lifeTime <= 0) {
            destroy();
            return;
        }

        if (stickToPlayer < 255) {
            PlayerState player;
            if (gameState.players.TryGetValue(stickToPlayer, out player)) {
                projectileObject.transform.position = player.currentCF.position;
                return;
            }

        }

        Vector3 newVelocity = vel; //put back new value at the end of method
        float speed = newVelocity.magnitude; //cache var
        if ((speed <= 0.5f) && (position.y <= 0.2f))
            return;

        RaycastHit hitInfo; //to check collisions
        Vector3 currentPos = projectileObject.transform.position; //cache var

        switch (typeID) {
            case BaboProjectileType.PROJECTILE_ROCKET:
                if (speed > 10) {
                    newVelocity /= speed;
                    speed = 10;
                    newVelocity *= speed;
                }
                position += newVelocity * delay;
                newVelocity += newVelocity * delay * 3;
                break;
            case BaboProjectileType.PROJECTILE_DROPED_WEAPON:
            case BaboProjectileType.PROJECTILE_DROPED_GRENADE:
            case BaboProjectileType.PROJECTILE_LIFE_PACK:
            case BaboProjectileType.PROJECTILE_GRENADE:

                position += newVelocity * delay;
                newVelocity.y -= GlobalServerVariables.GRAVITY * delay;
                if (Physics.Raycast(currentPos, newVelocity, out hitInfo, Vector3.Distance(currentPos, position))) {
                    position = position + hitInfo.normal * .01f;
                    newVelocity = Vector3.Reflect(newVelocity, hitInfo.normal);
                    newVelocity *= .65f;
                }
                break;
            case BaboProjectileType.PROJECTILE_COCKTAIL_MOLOTOV:
                position += newVelocity * delay;
                newVelocity.y -= GlobalServerVariables.GRAVITY * delay;
                break;
            case BaboProjectileType.PROJECTILE_FLAME:
                position += newVelocity * delay;
                newVelocity.y -= GlobalServerVariables.GRAVITY * delay;
                if (Physics.Raycast(currentPos, newVelocity, out hitInfo, Vector3.Distance(currentPos, position))) {
                    position = position + hitInfo.normal * .01f;
                    newVelocity = Vector3.zero;
                }
                break;
            default:
                if (Debug.isDebugBuild)
                    Debug.Log("Unexpected projectile: " + typeID.ToString());
                return;
        }
        /*if (Debug.isDebugBuild)
            Debug.LogFormat("ID {4} speed: {0} delay: {1} vel: {2} pos: {3}", speed, delay, vel.ToString(), position.ToString(), uniqueID);*/
        this.vel = newVelocity;
        projectileObject.transform.position = position;
    }
}


