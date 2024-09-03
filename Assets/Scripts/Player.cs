using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigid;
    private float forceJump = 0.2f;
    private float moveSpeed = 0.1f;
    private float bounceWallForce = 0.02f;
    private float dashForce = 0.2f;
    private Vector3 movingVect;
    private float slideRotationSpeed = 200f;
    private float slideInterval = 0.1f;
    private float slideAngle = 45f;
    private bool isSliding, isFlipping,isBunnyHoping,isGrounded;
    private float bunnyHopValue = 0.2f;
    private float momentumValue = 0.02f;
  
    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement() {
        movingVect = new Vector3(0, 0, 0);
        if (isGrounded) {
            if (Input.GetKey(KeyCode.D)) {
                rigid.MovePosition(new Vector3(transform.position.x, transform.position.y, transform.position.z + moveSpeed));
                movingVect = new Vector3(0, 0, 1);
            } else if (Input.GetKey(KeyCode.A)) {
                rigid.MovePosition(new Vector3(transform.position.x, transform.position.y, transform.position.z - moveSpeed));
                movingVect = new Vector3(0, 0, -1);
            } else if (Input.GetKey(KeyCode.W)) {
                rigid.MovePosition(new Vector3(transform.position.x - moveSpeed, transform.position.y, transform.position.z));
                movingVect = new Vector3(-1, 0, 0);
            } else if (Input.GetKey(KeyCode.S)) {
                rigid.MovePosition(new Vector3(transform.position.x + moveSpeed, transform.position.y, transform.position.z));
                movingVect = new Vector3(1, 0, 0);
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                JumpWithMomentum();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) && !isFlipping&&!isSliding&&!isBunnyHoping) {
                StartCoroutine(DoFlip(2f));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                Dash();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && !isSliding&&!isFlipping&&!isBunnyHoping) {
                StartCoroutine(Slide());
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && !isBunnyHoping&&!isFlipping&&!isSliding) {
                StartCoroutine(BunnyHop());
            }
        }

    }

    private void JumpWithMomentum() {
        rigid.AddForce(Vector3.up * forceJump, ForceMode.Impulse);
        rigid.AddForce(movingVect * momentumValue, ForceMode.Impulse);
    }

    private void Dash() {
        rigid.AddForce(movingVect * dashForce, ForceMode.Impulse);
    }

    private IEnumerator BunnyHop() {
        isBunnyHoping = true;
        transform.rotation = Quaternion.identity;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(new Vector3(slideAngle, transform.eulerAngles.y, transform.eulerAngles.z));
        while (transform.rotation != endRotation) {
            transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, Time.deltaTime * slideRotationSpeed);
            yield return new WaitForSecondsRealtime(slideInterval);
        }
        rigid.AddForce(Vector3.up * forceJump, ForceMode.Impulse);
        rigid.AddForce(movingVect * dashForce*bunnyHopValue, ForceMode.Impulse);
        while (transform.rotation != Quaternion.Euler(new Vector3(-slideAngle, transform.eulerAngles.y, transform.eulerAngles.z))) {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(-slideAngle, transform.eulerAngles.y, transform.eulerAngles.z)), Time.deltaTime * slideRotationSpeed);
            yield return new WaitForSecondsRealtime(slideInterval);
        }
        while (transform.rotation != startRotation) {
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * slideRotationSpeed);
            yield return new WaitForSecondsRealtime(slideInterval);
        }
        isBunnyHoping = false;
    }
    private IEnumerator Slide() {
        isSliding = true;
        transform.rotation = Quaternion.identity;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(new Vector3(slideAngle, transform.eulerAngles.y, transform.eulerAngles.z));
        rigid.AddForce(movingVect * dashForce, ForceMode.Impulse);
        while (transform.rotation != endRotation) {
            transform.rotation = Quaternion.Lerp(transform.rotation, endRotation, Time.deltaTime * slideRotationSpeed);
            yield return new WaitForSecondsRealtime(slideInterval);
        }
        while (transform.rotation != startRotation) {
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * slideRotationSpeed);
            yield return new WaitForSecondsRealtime(slideInterval);
        }
        isSliding = false;

    }
   
    IEnumerator DoFlip(float duration) {
        isFlipping = true;
        transform.rotation = Quaternion.identity;
        rigid.AddForce(Vector3.up * forceJump, ForceMode.Impulse);
        float startRotation = transform.eulerAngles.z;
        float endRotation = startRotation + 360.0f;
        float currentTime = 0f;
        while (currentTime < duration) {
            currentTime += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, currentTime / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zRotation);
            yield return null;
        }
        isFlipping = false;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Wall") {
            rigid.AddForce(collision.gameObject.transform.forward * bounceWallForce, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.transform.tag == "Ground") {
            isGrounded = true;
        } 
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.transform.tag == "Ground") {
            isGrounded = false;
        }
    }


}
