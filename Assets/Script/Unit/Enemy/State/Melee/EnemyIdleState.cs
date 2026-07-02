using System.Collections;
using UnityEngine;
using MyGame.Utility;

public class EnemyIdleState : EnemyBaseState<MeleeEnemy>
{
    private Coroutine detectCoroutine;
    private static readonly WaitForSeconds detecInterval = new WaitForSeconds(1f);
    public EnemyIdleState(MeleeEnemy enemy, EnemyStateMachine<MeleeEnemy> stateMachine) : base(enemy, stateMachine)
    {
        // 상태 초기화
    }

    public override void Enter()
    {
        // 상태 진입 시 로직
        Debugger.Log($"{enemy.name} entered Idle State.");

        // 코루틴 실행
        detectCoroutine = enemy.StartCoroutine(DetectPlayerCoroutine());
    }

    public override void Update()
    {
        // 상태 업데이트 로직
    }

    public override void Exit()
    {
        // 상태 종료 로직

        if(detectCoroutine != null)
        {
            // 코루틴 종료
            enemy.StopCoroutine(detectCoroutine);
            detectCoroutine = null;    
        }    
    }

    private IEnumerator DetectPlayerCoroutine()
    {
        while (true)
        {
            yield return detecInterval; // 1초(인터벌)마다 플레이어 감지

            // 플레이어 감지 로직
            // DetectRange 내에 플레이어가 있는지 확인함.
            Collider[] hits = Physics.OverlapSphere(enemy.transform.position, enemy.DetectRange, enemy.TargetLayerMask);
            if(hits.Length > 0)
            {
                // 장애물 확인.
                if(!DetectObstacles(hits[0].transform))
                {
                    // 플레이어 감지 됨 -> 타겟 설정.
                    enemy.SetTarget(hits[0].transform);
                    stateMachine.ChangeState<EnemyChaseState>();
                    yield break;        // 코루틴 종료.
                }

                // 장애물이 존재해 플레이어를 감지하지 못함.
            }
        }      
    }

    private bool DetectObstacles(Transform target)
    {
        Vector3 dirToTarget = (target.position - enemy.transform.position).normalized;
        float distToTarget = Vector3.Distance(enemy.transform.position, target.position);

        // 장애물 탐지
        if(Physics.Raycast(enemy.transform.position, dirToTarget, out RaycastHit hit, distToTarget, enemy.ObstacleLayerMask))
        {
            // 레이케스트에 무언가 맞음 -> 장애물이 존재함.
            return true;
        }
        
        // 장애물이 없음
        return false;
    }
}