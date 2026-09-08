using System;
using System.Collections;
using System.IO;
using LinhGioi.World;
using UnityEngine;
using UnityEngine.UIElements;

namespace LinhGioi.UI
{
    public sealed partial class OnboardingBlockoutPreview
    {
        private void UpdateTrainingSquare()
        {
            if(_stoneCompleted && IsNear(_world.RestLocation)) _forecourtVisited=true;
            _guidanceScroll.style.display=Dialogue.Active?DisplayStyle.None:DisplayStyle.Flex;
            _guidance.Objective.text=_forecourtVisited?"Khám phá Linh Thành.":_stoneCompleted?"Ghé trà đình.":_session.Completed?"Chạm Đá Luyện.":"Gặp Người Giữ Cổng.";
            _guidance.Hint.text=_forecourtVisited?"Bạn có thể nghỉ chân hoặc đi dạo quanh quảng trường.":_stoneCompleted
                ? OnboardingDialogueContent.StoneFeedback(Time.unscaledTime-_stoneFeedbackStartedAt) ?? "Theo vòng sân sang phải, tới trà đình."
                :_session.Completed?"Tới dấu sáng trên mép bệ phía trước.":"Người Giữ Cổng ở bên trái lối vào.";
            var place = NearbySquarePlace();
            if (place >= 0 && !Dialogue.Active)
            {
                _guidance.Objective.text = place == 0 ? "Trà đình" : "Gian nghề";
                _guidance.Hint.text = _placesRead[place]
                    ? (place == 0 ? "Bạn có thể xem lại bộ trà; gian nghề ở phía bắc, dọc mép sân." : "Bạn có thể xem lại dụng cụ hoặc trở về quảng trường.")
                    : (place == 0 ? "Một chỗ dừng chân dưới mái ngói. Nhấn F hoặc Xem để quan sát bộ trà." : "Bàn nghề bên quảng trường. Nhấn F hoặc Xem để quan sát dụng cụ.");
            }
            _world.SetSquarePlaceFocus(place, !Dialogue.Active);
            var returnGuide=_stoneCompleted && InRange;
            RuntimeUiFactory.ApplyWorldTouchInteraction(_interact,place>=0?"Xem":returnGuide?"Gặp":_stoneCompleted?"Đã xong":_session.Completed?"Luyện":"Gặp");
            _interact.SetEnabled(!Dialogue.Active && (place>=0 || (InRange && (!_stoneCompleted || returnGuide))));
            _world.KeeperReady=(!_session.Completed || _stoneCompleted) && InRange;
            _world.SetStoneFeedback(_session.Completed && !_stoneCompleted && InRange,_stoneCompleted);
            if(!_capturing) _world.ScreenMovement=Application.isFocused?_pad.Value:Vector2.zero;
            if(Input.GetKeyDown(KeyCode.F)) Interact();
            if(Input.GetKeyDown(KeyCode.Escape)) HandleEscape();
        }

        private static void CheckSquareLandmarkFrame(Renderer stone)
        {
            var bounds=stone.bounds;
            foreach(var y in new[]{bounds.min.y,bounds.max.y})
            {
                var point=Camera.main.WorldToViewportPoint(new Vector3(bounds.center.x,y,bounds.center.z));
                if(point.z<=0 || point.y<.03f || point.y>.97f)
                    throw new InvalidOperationException("Gameplay camera clipped landmark: viewport="+point);
            }
        }

        private IEnumerator CaptureTrainingSquareReview(string directory)
        {
            if(!Debug.isDebugBuild || !_world.TrainingSquare) throw new InvalidOperationException("Square review needs square development candidate.");
            _capturing=true;directory=Path.GetFullPath(directory);Directory.CreateDirectory(directory);
            yield return VisualRuntimeEvidenceRunner.ResizePlayerViewport(1920,1080);
            yield return new WaitForSeconds(1f);
            var stone=_world.transform.Find("Blockout Stone");var mesh=stone.GetComponent<MeshFilter>().sharedMesh;
            if(Mathf.Abs(mesh.bounds.size.y-6.5f)>.01f || mesh.bounds.size.x<4.5f || stone.GetComponent<MeshCollider>()==null)
                throw new InvalidOperationException("Square landmark scale/collision is wrong.");
            foreach(var vertex in mesh.vertices)
                if(float.IsNaN(vertex.sqrMagnitude) || float.IsInfinity(vertex.sqrMagnitude))
                    throw new InvalidOperationException("Landmark has non-finite geometry.");
            foreach(var material in stone.GetComponent<Renderer>().sharedMaterials)
                if(material.shader.name!="LGO/TrainingStoneSurface" || !material.shader.isSupported)
                    throw new InvalidOperationException("Landmark surface failed in Player.");
            CheckSquareLandmarkFrame(stone.GetComponent<Renderer>());
            yield return Capture(directory,"arrival-gate");
            yield return WalkTo(new Vector3(-3.7f,0,-20));
            Submit(_interact);
            if(!Dialogue.Active) throw new InvalidOperationException("Square guide is not reachable.");
            yield return Capture(directory,"guide");
            for(var step=0;step<3;step++)Submit(_dialogue.ContinueButton);
            if(!_session.Completed || Dialogue.Active) throw new InvalidOperationException("Square guide did not release input.");
            yield return WalkTo(new Vector3(0,0,-14));
            yield return Capture(directory,"square-approach");
            yield return WalkTo(new Vector3(0,0,-3.8f));
            CheckSquareLandmarkFrame(stone.GetComponent<Renderer>());
            yield return Capture(directory,"stone-nearby");
            var materialCrystal=stone.GetComponent<Renderer>().sharedMaterials[2];
            var idle=materialCrystal.GetColor("_EmissionColor");
            Submit(_interact);
            var deadline=Time.realtimeSinceStartup+4f;
            do { yield return new WaitForEndOfFrame(); }
            while(materialCrystal.GetColor("_EmissionColor").b<=idle.b*1.5f && Time.realtimeSinceStartup<deadline);
            if(!_stoneCompleted || materialCrystal.GetColor("_EmissionColor").b<=idle.b*1.5f)
                throw new InvalidOperationException("Landmark touch did not produce resonance.");
            yield return Capture(directory,"stone-pulse");
            yield return new WaitForSeconds(2.7f);yield return new WaitForEndOfFrame();
            if(Vector4.Distance(idle,materialCrystal.GetColor("_EmissionColor"))>.001f)
                throw new InvalidOperationException("Landmark resonance failed to settle.");
            var stamp=_stoneFeedbackStartedAt;Interact();
            if(stamp!=_stoneFeedbackStartedAt) throw new InvalidOperationException("Landmark replayed completion.");
            yield return Capture(directory,"stone-settled");
            foreach(var point in new[]{new Vector3(-6,0,-5),new Vector3(-9,0,0),new Vector3(-9,0,9),new Vector3(0,0,12),new Vector3(9,0,9),new Vector3(12,0,4)})
                yield return WalkTo(point);
            if(!_forecourtVisited || Mathf.Abs(_world.Position.y)>.1f)
                throw new InvalidOperationException("Square circulation/tea route failed.");
            yield return Capture(directory,"tea-arrival");
            GetComponent<UIDocument>().rootVisualElement.style.visibility=Visibility.Hidden;
            foreach(var label in _world.GetComponentsInChildren<TextMesh>(true))
                if(!label.name.StartsWith("Square sign",StringComparison.Ordinal))label.GetComponent<Renderer>().enabled=false;
            _world.BeginArchitectureReview();
            var positions=new[]{new Vector3(-14,14,-20),new Vector3(12,3,7),new Vector3(10.5f,2.2f,.2f),new Vector3(9,2.6f,10.5f),new Vector3(13.3f,1.05f,11.6f)};
            for(var view=0;view<positions.Length;view++)
            {
                Camera.main.transform.position=positions[view];Camera.main.transform.LookAt(view==4?new Vector3(16.5f,.38f,14f):view==3?new Vector3(12,1.5f,16):view==2?new Vector3(15,1.65f,4):new Vector3(0,2.8f,0));
                yield return Capture(directory,"square-inspection-"+view);
            }
            Debug.Log("LGO_TRAINING_SQUARE_CAPTURE_COMPLETE frames=12 landmark_triangles="+mesh.triangles.Length/3+" height=6.5 guide=true touch=true pulse=true settled=true repeat_guard=true circulation=true tea=true viewport="+Screen.width+"x"+Screen.height);
            Application.Quit(0);
        }
    }
}
