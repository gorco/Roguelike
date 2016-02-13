using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour
{
	public float moveTime = 0.1f;           //Time it will take object to move, in seconds.
	public LayerMask blockingLayer;         //Layer on which collision will be checked.


	private BoxCollider2D boxCollider;      //The BoxCollider2D component attached to this object.
	private Rigidbody2D rb2D;               //The Rigidbody2D component attached to this object.
	private float inverseMoveTime;          //Used to make movement more efficient.

	public int life;                     //Used to store player life points total during level.
	public int maxLife;
	public int str;
	public int def;
	public int dex;
	public int spd;
	public int luc;

	protected virtual void Start()
	{
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();

		//By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
		inverseMoveTime = 1f / moveTime;
	}

	//Move returns true if it is able to move and false if not. 
	//Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
	protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
	{
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2(xDir, yDir);

		boxCollider.enabled = false;

		//Cast a line from start point to end point checking collision on blockingLayer.
		hit = Physics2D.Linecast(start, end, blockingLayer);
		RaycastHit2D tmpHit = hit;

		if (boxCollider.size.x > 1 || boxCollider.size.y > 1)
		{
			Vector2 rayEnd = new Vector2(0, 0);
			Transform player = GameObject.Find("Player").transform;
            if (yDir != 0)
			{
				for (int i = -Mathf.RoundToInt(boxCollider.size.x) / 2; i < boxCollider.size.x / 2; i++)
				{
					start.x = transform.position.x + i;
					rayEnd = start + new Vector2(xDir, yDir * Mathf.Ceil(boxCollider.size.y / 2));
					tmpHit = Physics2D.Linecast(start, rayEnd, blockingLayer);
					if (tmpHit.transform == player)
					{
						hit = tmpHit;
                        break;
					} else if (tmpHit.transform != null)
					{
						hit = tmpHit;
					}
				}
			}
			else if (xDir != 0)
			{
				for (int i = -Mathf.RoundToInt(boxCollider.size.y) / 2; i < boxCollider.size.y / 2; i++)
				{
					start.y = transform.position.y + i;
					rayEnd = start + new Vector2(xDir * Mathf.Ceil(boxCollider.size.x / 2), yDir);
					tmpHit = Physics2D.Linecast(start, rayEnd, blockingLayer);
					if (tmpHit.transform == player)
					{
						hit = tmpHit;
						break;
					}
					else if (tmpHit.transform != null)
					{
						hit = tmpHit;
					}
				}
			}
		}

		boxCollider.enabled = true;

		if (hit.transform == null)
		{
			//If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
			StartCoroutine(SmoothMovement(end));
			return true;
		}

		return false;
	}


	//Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
	protected IEnumerator SmoothMovement(Vector3 end)
	{
		//Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
		//Square magnitude is used instead of magnitude because it's computationally cheaper.
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while (sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPostion);

			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			yield return null;
		}
	}


	//The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
	//AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
	protected virtual void AttemptMove<T>(int xDir, int yDir)
		where T : Destuctible
	{
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);

		if (hit.transform == null)
			return;

		T hitComponent = hit.transform.GetComponent<T>();

		if (!canMove && hitComponent != null)
			OnCantMove(hitComponent);
	}

	//The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
	//OnCantMove will be overriden by functions in the inheriting classes.
	protected abstract void OnCantMove<T>(T component)
		where T : Destuctible;
}

