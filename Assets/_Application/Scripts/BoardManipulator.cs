using GMTK2020.Data;
using GMTK2020.Rendering;
using GMTK2020.UI;
using System;
using UnityEngine;

namespace GMTK2020
{
	public class BoardManipulator : MonoBehaviour
	{
		[SerializeField] private SwapDisplay swapDisplay = null;
		[SerializeField] private BoardRenderer boardRenderer = null;
		[SerializeField] private MouseEventSource mouseEventSource = null;

		private int _remainingSwaps = 0;
		private int RemainingSwaps
		{
			get => _remainingSwaps;
			set
			{
				_remainingSwaps = value;
				swapDisplay.SetSwaps(RemainingSwaps);
			}
		}

		private bool boardLocked = true;
		private Tile[,] grid;
		private int width;
		private int height;

		private Tile draggedTile = null;
		private Vector2Int draggedFrom;
		private Tile swappedTile = null;
		private Vector2Int swapDir;

		private void Awake()
		{
			mouseEventSource.DragStartedAt += OnDragStarted;
			mouseEventSource.DraggedTo += OnDraggedTo;
			mouseEventSource.DragStoppedAt += OnDragStopped;
		}

		private void OnDestroy()
		{
			mouseEventSource.DragStartedAt -= OnDragStarted;
			mouseEventSource.DraggedTo -= OnDraggedTo;
			mouseEventSource.DragStoppedAt -= OnDragStopped;
		}

		public void Initialize(Tile[,] initialGrid)
		{
			grid = initialGrid;

			width = initialGrid.GetLength(0);
			height = initialGrid.GetLength(1);

			UnlockBoard();
		}

		public void LockBoard()
		{
			boardLocked = true;
		}

		public void UnlockBoard()
		{
			boardLocked = false;
		}

		public void GrantSwap()
		{
			++RemainingSwaps;
		}

		private void UseUpSwap()
		{
			--RemainingSwaps;
		}

		private void OnDragStarted(Vector2 fromPos)
		{
			if (boardLocked || RemainingSwaps == 0)
				return;

			Vector2Int? gridPosOrNull = boardRenderer.PixelSpaceToGridCoordinates(fromPos);

			if (gridPosOrNull is null)
				return;

			draggedFrom = gridPosOrNull.Value;
			draggedTile = grid[draggedFrom.x, draggedFrom.y];
		}

		private void OnDraggedTo(Vector2 dragPos)
		{
			if (draggedTile is null)
				return;

			Vector2Int newSwapDir;

			Vector2 localDragPos = boardRenderer.PixelSpaceToLocalCoordinates(dragPos);
			Vector2 relativeDragPos = localDragPos - draggedFrom;

			if (relativeDragPos.x > Math.Abs(relativeDragPos.y))
				newSwapDir = Vector2Int.right;
			else if (relativeDragPos.y > Math.Abs(relativeDragPos.x))
				newSwapDir = Vector2Int.up;
			else if (-relativeDragPos.x > Math.Abs(relativeDragPos.y))
				newSwapDir = Vector2Int.left;
			else
				newSwapDir = Vector2Int.down;

			Vector2Int swapTo = draggedFrom + newSwapDir;

			Tile newSwappedTile;
			if (swapTo.x < 0 || swapTo.y < 0 || swapTo.x >= width || swapTo.y >= height)
				newSwappedTile = null;
			else
				newSwappedTile = grid[swapTo.x, swapTo.y];

			if (swappedTile != null && newSwappedTile != swappedTile)
				boardRenderer.ResetPosition(swappedTile, draggedFrom + swapDir);

			if (newSwappedTile is null)
				boardRenderer.ResetPosition(draggedTile, draggedFrom);
			else
			{
				float swapDistance = Mathf.Clamp01(Vector2.Dot(relativeDragPos, swapDir));

				boardRenderer.RenderPartialSwap(draggedTile, draggedFrom, newSwappedTile, draggedFrom + newSwapDir, swapDistance);
			}

			swappedTile = newSwappedTile;
			swapDir = newSwapDir;
		}

		private void OnDragStopped(Vector2 toPos)
		{
			if (draggedTile is null)
				return;

			if (swappedTile is null)
			{
				boardRenderer.ResetPosition(draggedTile, draggedFrom);
			}
			else
			{
				Vector2 localDragPos = boardRenderer.PixelSpaceToLocalCoordinates(toPos);
				float swapDistance = Mathf.Clamp01(Vector2.Dot(localDragPos - draggedFrom, swapDir));

				if (swapDistance <= 0.5f)
				{
					boardRenderer.ResetPosition(draggedTile, draggedFrom);
					boardRenderer.ResetPosition(swappedTile, draggedFrom + swapDir);
				}
				else
				{
					grid[draggedFrom.x, draggedFrom.y] = swappedTile;
					grid[draggedFrom.x + swapDir.x, draggedFrom.y + swapDir.y] = draggedTile;

					boardRenderer.RenderCompletedSwap(draggedTile, draggedFrom, swappedTile, draggedFrom + swapDir);

					UseUpSwap();
				}
			}

			draggedTile = null;
			swappedTile = null;
		}
	}
}
