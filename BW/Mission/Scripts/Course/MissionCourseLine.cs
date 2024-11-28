using Gongju.Web;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissionCourseLine : MonoBehaviour
{
    [System.Serializable]
    public class CourseLine
    {
        public int missionId;
        public Transform courseParent;
        public List<Transform> coursePositions;
    }

    [SerializeField] private List<CourseLine> missionCourses = new List<CourseLine>();
    [SerializeField, ReadOnly] private LineRenderer currentCourceLine;
    [SerializeField] private float intervalDistance  = 1f;

    private void Awake()
    {
        foreach (CourseLine course in missionCourses) {
            course.coursePositions = new List<Transform>();

            foreach (Transform point in course.courseParent) {
                course.coursePositions.Add(point);
            }
        }
    }

    private void OnEnable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.courseMissionManager.MissionCourseLine = this;
        }
    }

    private void OnDisable()
    {
        if (MissionManager.instance)
        {
            MissionManager.instance.courseMissionManager.MissionCourseLine = null;
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        if (!GameManager.instance.isGuest) {
            yield return new WaitUntil(() => MissionManager.instance.isMissionInit);
            CourseLineRenderer(DatabaseConnector.instance.memberData.current_mission_idx);
        }
    }

    public void CourseLineRenderer(int missionId)
    {
        // Remove Before LineRenderer
        if (currentCourceLine != null) Destroy(currentCourceLine.gameObject);

        // Finish Or Completed => Return
        var missionData = MissionManager.instance.GetMission(missionId);
        if (missionData == null) return;

        var progressData = MissionManager.instance.GetMissionProgress(missionId);
        var routesType = MissionManager.instance.GetMissionRoutesType(missionId);
        if (routesType == "completed" || routesType == "finished" || progressData.missionProgress < 0) return;

        // Get Mission
        var missionCourse = missionCourses.Find(x => x.missionId == missionId);

        // Null => Return 
        if (missionCourse == null) return;

        // Get Interpolated Position Value  
        List<Vector3> interpolatedPositions = InterpolatedPosition(missionCourse);
        
        // Set LineRenderer
        SetLineRenderer(interpolatedPositions);
    }

    private List<Vector3> InterpolatedPosition(CourseLine missionCourse)
    {
        List<Vector3> interpolatedPositions = new List<Vector3>();

        for (int i = 0; i < missionCourse.coursePositions.Count - 1; ++i) {
            Vector3 start = missionCourse.coursePositions[i].position;
            Vector3 end = missionCourse.coursePositions[i + 1].position;
            float distance = Vector3.Distance(start, end);
            
            interpolatedPositions.Add(start);

            Vector3 direction = (end - start).normalized;
            float coveredDistance = 0f;
            
            while (coveredDistance + intervalDistance < distance) {
                coveredDistance += intervalDistance;
                Vector3 interpolatedPos = start + direction * coveredDistance;
                interpolatedPositions.Add(interpolatedPos);
            }
        }
        interpolatedPositions.Add(missionCourse.coursePositions[missionCourse.coursePositions.Count - 1].position);

        return interpolatedPositions;
    }

    private void SetLineRenderer(List<Vector3> positions)
    {
        LineRenderer lineRenderer = Instantiate(Resources.Load<LineRenderer>("CourseLineRenderer"));
        lineRenderer.transform.SetParent(this.transform);

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.Select(x => new Vector3(x.x, x.z, x.y * -1f)).ToArray());

        currentCourceLine = lineRenderer;
    }
}