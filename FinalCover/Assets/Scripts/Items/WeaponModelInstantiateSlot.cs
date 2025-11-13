using UnityEngine;

public class WeaponModelInstantiateSlot : MonoBehaviour
{
    public WeaponModelSlot weaponSlot;
    public GameObject currentWeaponModel;

    public void LoadWeapon(GameObject weaponModel)
    {
        currentWeaponModel = weaponModel;
        weaponModel.transform.parent = transform;

        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
    }

    public void UnloadWeapon()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }
}
