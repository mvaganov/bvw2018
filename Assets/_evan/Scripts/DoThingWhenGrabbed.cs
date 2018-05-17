namespace VRTK {

	using UnityEngine;

	public class DoThingWhenGrabbed : VRTK_InteractableObject {
		public UnityEngine.Events.UnityEvent evGrabbed;
		public UnityEngine.Events.UnityEvent evUsed;

		public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
		{
			Debug.Log ("Grabbed");
			evGrabbed.Invoke ();
			base.OnInteractableObjectGrabbed (e);

		}

		public override void OnInteractableObjectUsed(InteractableObjectEventArgs e)
		{
			Debug.Log ("Used");
			evGrabbed.Invoke ();
			base.OnInteractableObjectUsed (e);

		}
	}
}
