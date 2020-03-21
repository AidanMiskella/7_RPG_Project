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

        Fighter fighter;
        GameObject player;
        Health health;
        Mover mover;

        Vector3 guardLocation;
        float timeSinceLastSawPlayer = Mathf.Infinity;

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

                GuardBehaviour();
            }

            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void GuardBehaviour() {

            mover.StartMoveAction(guardLocation);
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