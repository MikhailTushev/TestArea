using NUnit.Framework;

namespace TestArea.UnitTests;

public class AddTwoNumbersTests
{
    [Test]
    public void AddTwoNumbers()
    {
        var l1 = new ListNode(2, new ListNode(4, new ListNode(3)));
        var l2 = new ListNode(5, new ListNode(6, new ListNode(4)));

        var result = new ListNode();
        AddTwoNumbersRec(result, l1, l2);
    }

    [Test]
    public void AddTwoNumbersDiff()
    {
        var l1 = new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9))))));
        var l2 = new ListNode(9, new ListNode(9, new ListNode(9, new ListNode(9))));

        var result = new ListNode();
        AddTwoNumbersRec(result, l1, l2);
    }


    private void AddTwoNumbersRec(ListNode rs, ListNode l1, ListNode l2)
    {
        var result = rs.val;
        if (l1 is not null)
        {
            result += l1.val;
        }

        if (l2 is not null)
        {
            result += l2.val;
        }

        var result1 = rs.val + (l1?.val ?? 0) + (l2?.val ?? 0);

        if (result >= 10)
        {
            rs.next = new ListNode();
            rs.val = result % 10;
            rs.next.val = result / 10;
        }
        else
        {
            rs.val = result;
        }

        if (l1?.next is null && l2?.next is null)
        {
            return;
        }

        rs.next ??= new ListNode();
        AddTwoNumbersRec(rs.next, l1?.next, l2?.next);
    }

    public class ListNode
    {
        public int val;
        public ListNode next;

        public ListNode(int val = 0, ListNode next = null)
        {
            this.val = val;
            this.next = next;
        }
    }
}