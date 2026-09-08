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
        private NpcDialogueSession _placeSession;
        private int _readingPlace;
        private readonly bool[] _placesRead = new bool[2];

        private int NearbySquarePlace()
        {
            for (var i=0;i<2;i++) if (IsNear(OnboardingBlockoutWorld.SquarePlaceLocation(i))) return i;
            return -1;
        }

        private bool TryOpenSquarePlace()
        {
            var place=NearbySquarePlace();
            if(place<0) return false;
            _readingPlace=place;
            _placeSession=place==0
                ? new NpcDialogueSession("Trà đình",new[]{
                    "Một bộ ấm chén đặt giữa bàn gỗ. Những chiếc ghế dài hướng vào nhau dưới mái ngói, vừa đủ cho một cuộc chuyện trò.",
                    "Từ đây có thể nhìn về Đá Luyện. Men theo mép sân phía bắc là gian nghề; lối giữa quảng trường vẫn rộng cho người qua lại."
                },"Quan sát bộ trà","Ấm và chén nhỏ được đặt gọn giữa bàn, chừa khoảng trống quanh mép. Mái hiên và hàng cây che bớt nắng; đây là chỗ dừng chân giữa hành trình.","Khám phá tiếp")
                : new NpcDialogueSession("Gian nghề",new[]{
                    "Giá búa nằm sau bàn nghề; khối kê và những phôi nhỏ được đặt riêng trên mặt bàn. Mỗi nhóm dụng cụ có một chỗ dễ tìm.",
                    "Linh Thành có chỗ cho người đi đường, người làm nghề và người dùng thuật. Võ, Kiếm, Pháp, Cơ hay Linh đều cùng đi qua quảng trường này."
                },"Quan sát dụng cụ","Các cán búa được treo riêng trên giá. Những vật liệu nhỏ gom trong khay giúp dễ lựa chọn và giữ lối quanh bàn thông thoáng.","Khám phá tiếp");
            _placeSession.Open();
            _world.PlaceLookTarget=place==0?new Vector3(15,.7f,4):new Vector3(12,.9f,16);
            _world.PlaceViewPosition=place==0?new Vector3(12.1f,1.8f,1.8f):new Vector3(9.6f,2f,13.6f);
            RefreshDialogue();
            return true;
        }

        private void CloseDialogue()
        {
            Dialogue.Close();
            _placeSession=null;
            RefreshDialogue();
        }

        private IEnumerator CaptureSquarePlacesReview(string directory)
        {
            if(!Debug.isDebugBuild || !_world.TrainingSquare) throw new InvalidOperationException("Place review needs square development candidate.");
            _capturing=true;
            directory=Path.GetFullPath(directory);Directory.CreateDirectory(directory);
            yield return VisualRuntimeEvidenceRunner.ResizePlayerViewport(1920,1080);
            yield return new WaitForSeconds(1);
            // Optional exploration is available before the stone introduction, with no progress bypass.
            foreach(var point in new[]{new Vector3(9,0,-20),new Vector3(10,0,-5),new Vector3(12,0,4)})
                yield return WalkTo(point);
            for(var place=0;place<2;place++)
            {
                if(place==1)
                    foreach(var point in new[]{new Vector3(10,0,6),new Vector3(10,0,12),new Vector3(12,0,14.5f)})
                        yield return WalkTo(point);
                yield return new WaitForEndOfFrame();
                var name=place==0?"tea":"craft";
                if(NearbySquarePlace()!=place || !_interact.enabledSelf || _interact.text!="Xem")
                    throw new InvalidOperationException("Place approach action missing: "+name);
                yield return Capture(directory,name+"-nearby");
                Submit(_interact);
                yield return new WaitForSeconds(.3f);
                if(_placeSession==null || !_world.PlaceReadingVisible || _world.DialogueVisible || _dialogue.Portrait.resolvedStyle.display!=DisplayStyle.None)
                    throw new InvalidOperationException("Place inspection incorrectly treated as NPC dialogue.");
                var root=GetComponent<UIDocument>().rootVisualElement;
                if(_dialogue.Panel.worldBound.xMin>root.worldBound.width*.1f ||
                    _dialogue.Panel.worldBound.xMax>root.worldBound.width*.5f)
                    throw new InvalidOperationException("Place panel obscures the object inspection area.");
                var position=_world.Position;
                _world.Movement=Vector2.up;
                yield return new WaitForSeconds(.25f);
                _world.Movement=Vector2.zero;
                if(Vector3.Distance(position,_world.Position)>.03f) throw new InvalidOperationException("Reading leaked movement.");
                yield return Capture(directory,name+"-reading");
                HandleEscape();
                if(Dialogue.Active || _world.PlaceReadingVisible || _placesRead[place])
                    throw new InvalidOperationException("Closing a place granted completion or retained input lock.");
                yield return new WaitForEndOfFrame();
                Submit(_interact);
                if(_placeSession==null) throw new InvalidOperationException("Place did not reopen.");
                Submit(_dialogue.ContinueButton);
                Submit(_dialogue.InformationButton);
                if(_placeSession==null || !_placeSession.ReadingInformation) throw new InvalidOperationException("Place detail action failed.");
                yield return Capture(directory,name+"-details");
                Submit(_dialogue.ContinueButton);
                Submit(_dialogue.ContinueButton);
                if(!_placesRead[place] || Dialogue.Active || _world.PlaceReadingVisible || _session.Completed || _stoneCompleted)
                    throw new InvalidOperationException("Place completion altered introduction state.");
            }
            yield return WalkTo(new Vector3(9,0,14));
            if(NearbySquarePlace()>=0 || _interact.enabledSelf) throw new InvalidOperationException("Place action remained enabled outside range.");
            yield return Capture(directory,"exploration-resumed");
            Debug.Log("LGO_SQUARE_PLACES_CAPTURE_COMPLETE frames=7 tea=true craft=true optional_before_stone=true close_reopen=true input_restored=true npc_independent=true no_progress_bypass=true viewport="+Screen.width+"x"+Screen.height);
            Application.Quit(0);
        }
    }
}
