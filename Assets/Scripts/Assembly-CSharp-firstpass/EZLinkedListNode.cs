using System;

public class EZLinkedListNode<T> : IEZLinkedListItem<EZLinkedListNode<T>>
{
	public EZLinkedListNode(T v)
	{
		this.val = v;
	}

	public EZLinkedListNode<T> prev
	{
		get
		{
			return this.m_prev;
		}
		set
		{
			this.m_prev = value;
		}
	}

	public EZLinkedListNode<T> next
	{
		get
		{
			return this.m_next;
		}
		set
		{
			this.m_next = value;
		}
	}

	public T val;

	private EZLinkedListNode<T> m_prev;

	private EZLinkedListNode<T> m_next;
}
