using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {

    public class AIController : MonoBehaviour {

        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerence = 1f;
        [SerializeField] float waypointDwellTime = 3f;

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        Vector3 guardLocation;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArriveAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Start() {

            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");

            guardLocation = transform.position;
        }

        private void Update() {

            if (health.IsDead()) return;

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player)) {

                timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            } else if (timeSinceLastSawPlayer < suspicionTime) {

                SuspiciousBehaviour();
            } else {

                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers() {

            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArriveAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour() {

            Vector3 nextPosition = guardLocation;

            if (patrolPath != null) {

                if (AtWaypoint()) {

                    timeSinceArriveAtWaypoint = 0;
                    CycleWaypont();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArriveAtWaypoint > waypointDwellTime) {

                mover.StartMoveAction(nextPosition);
            }
        }

        private bool AtWaypoint() {

            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerence;
        }

        private void CycleWaypont() {


            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint() {

            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspiciousBehaviour() {

            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour() {

            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer() {

            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;
        }

        // Called by Unity
        private void OnDrawGizmosSelected() {

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}