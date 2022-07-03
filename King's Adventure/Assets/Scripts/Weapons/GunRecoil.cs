using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Rotations")]
    Vector3 currentRotation;
    Vector3 targetRotation;

    [Header("Hipfire Recoil")]
    [SerializeField] float recoilX = -2f;
    [SerializeField] float recoilY = 2f;
    [SerializeField] float recoilZ = 0.35f;

    [Header("Aiming Recoil")]
    [SerializeField] float aimRecoilX = -2f;
    [SerializeField] float aimRecoilY = 2f;
    [SerializeField] float aimRecoilZ = 0.35f;

    [Header("Settings")]
    [SerializeField] float snappieness = 6f;
    [SerializeField] float returnSpeed = 2f;
    [Space]
    bool isAiming;

    [Header("References")]
    [SerializeField] GunSystem gunScript;

    // Update is called once per frame
    void Update()
    {
        isAiming = gunScript.aiming;

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappieness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        if(isAiming)
        {
            targetRotation += new Vector3(aimRecoilX, Random.Range(-aimRecoilY, aimRecoilY), Random.Range(-aimRecoilZ, aimRecoilZ));
        }
        else
        {
            targetRotation += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        }
    }
}
