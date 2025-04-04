using NUnit.Framework;
using UnityEngine;

namespace CodeBase.Tests
{
    public class BoardServiceTests
    {
        [Test]
        public void ShouldCorrectlySetAndGetMarkerAtBoard()
        {
            // Arrange
            var boardService = new BoardService();
            boardService.CreateBoard(new Vector2Int(3, 3));
            var markerCross = Marker.Cross;
            var markerCircle = Marker.Circle;

            // Act
            boardService.TrySetMarker(0, 0, markerCross);
            boardService.TrySetMarker(1, 1, markerCircle);

            var resultOutOfBoardAny = boardService.TrySetMarker(5, 1, markerCross);
            var resultOutOfBoardSize = boardService.TrySetMarker(3, 3, markerCross);

            // Assert
            Assert.IsTrue(boardService.GetMarker(0, 0) == markerCross);
            Assert.IsTrue(boardService.GetMarker(1, 1) == markerCircle);
            Assert.IsFalse(resultOutOfBoardAny);
            Assert.IsFalse(resultOutOfBoardSize);
        }


        [Test]
        public void ShouldProperlyChangeMarker()
        {
            // Arrange
            var boardService = new BoardService();
            boardService.CreateBoard(new Vector2Int(3, 3));
            var markerCross = Marker.Cross;
            var markerCircle = Marker.Circle;

            // Act
            boardService.TrySetMarker(0, 0, markerCross);
            Assert.IsTrue(boardService.GetMarker(0, 0) == markerCross);

            // Assert
            boardService.TrySetMarker(0, 0, markerCircle);
            Assert.IsTrue(boardService.GetMarker(0, 0) == markerCircle);
        }


        [Test]
        public void ShouldDetectWinConditionWhenRequiredNumberOfMarkersInARow()
        {
            // Arrange
            var boardService = new BoardService();
            boardService.CreateBoard(new Vector2Int(3, 3));
            var marker = Marker.Cross;
            var targetCount = 3;

            // Act
            boardService.TrySetMarker(0, 0, marker);
            boardService.TrySetMarker(1, 0, marker);
            boardService.TrySetMarker(2, 0, marker);

            // Assert
            Assert.IsTrue(boardService.CheckFullMapForWin(marker, targetCount));

            // Arrange
            boardService.CreateBoard(new Vector2Int(5, 5));
            targetCount = 5;

            // Act
            boardService.TrySetMarker(0, 0, marker);
            boardService.TrySetMarker(1, 1, marker);
            boardService.TrySetMarker(2, 2, marker);
            boardService.TrySetMarker(3, 3, marker);
            boardService.TrySetMarker(4, 4, marker);

            // Assert
            Assert.IsTrue(boardService.CheckFullMapForWin(marker, targetCount));
        }


        [Test]
        public void ShouldReturnCorrectSequenceCountsForGivenPositionAndMarker()
        {
            // Arrange
            var boardService = new BoardService();
            boardService.CreateBoard(new Vector2Int(5, 5));
            var marker = Marker.Cross;
            var minCount = 3;
            var maxCount = 5;

            // Act
            // Center
            boardService.TrySetMarker(2, 2, marker);

            // Horizontal 5 in row
            boardService.TrySetMarker(3, 2, marker);
            boardService.TrySetMarker(4, 2, marker);
            boardService.TrySetMarker(1, 2, marker);
            boardService.TrySetMarker(0, 2, marker);

            // Vertical 4 in row
            boardService.TrySetMarker(2, 3, marker);
            boardService.TrySetMarker(2, 4, marker);
            boardService.TrySetMarker(2, 1, marker);

            // Diagonal 1 3 in row
            boardService.TrySetMarker(1, 1, marker);
            boardService.TrySetMarker(3, 3, marker);

            // Diagonal 2 3 in row
            boardService.TrySetMarker(1, 3, marker);
            boardService.TrySetMarker(3, 1, marker);
            // Assert
            var sequences = boardService.GetSequencesFromPosition(2, 2, marker, minCount, maxCount);
            Assert.AreEqual(2, sequences[0]); // 3
            Assert.AreEqual(1, sequences[1]); // 4
            Assert.AreEqual(1, sequences[2]); // 5
        }
    }
}