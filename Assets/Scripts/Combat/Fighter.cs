using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour, IAction {

        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 20f;

        Health target;
        Mover mover;
        Animator animator;
        float timeSinceLastAttack = 0;

        private void Start() {

            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
        }

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange()) {

                mover.MoveTo(target.transform.position);
            } else {

                mover.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour() {

            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= timeBetweenAttacks) {

                // Will trigger Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack() {

            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        // Animation event
        void Hit() {

            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }

        private bool GetIsInRange() {

            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public bool CanAttack(CombatTarget combatTarget) {

            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(CombatTarget combatTarget) {

            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel() {

            StopAttack();
            target = null;
        }

        private void StopAttack() {

            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }
    }
}

