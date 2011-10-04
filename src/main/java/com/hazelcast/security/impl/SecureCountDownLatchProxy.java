package com.hazelcast.security.impl;

import java.util.concurrent.TimeUnit;

import com.hazelcast.core.InstanceDestroyedException;
import com.hazelcast.core.Member;
import com.hazelcast.core.MemberLeftException;
import com.hazelcast.impl.CountDownLatchProxy;
import com.hazelcast.impl.Node;
import com.hazelcast.impl.monitor.CountDownLatchOperationsCounter;
import com.hazelcast.monitor.LocalCountDownLatchStats;
import com.hazelcast.nio.Address;
import com.hazelcast.security.SecurityConstants;
import com.hazelcast.security.permission.CountDownLatchPermission;

final class SecureCountDownLatchProxy extends SecureProxySupport implements CountDownLatchProxy {
	
	final CountDownLatchProxy proxy;
	
	SecureCountDownLatchProxy(Node node, final CountDownLatchProxy proxy) {
		super(node);
		this.proxy = proxy;
	}

	private void checkCountDown() {
		checkPermission(new CountDownLatchPermission(getName(), SecurityConstants.ACTION_COUNTDOWN));
	}
	
	private void checkSet() {
		checkPermission(new CountDownLatchPermission(getName(), SecurityConstants.ACTION_SET));
	}
	
	public boolean setCount(int count, Address ownerAddress) {
		checkSet();
		return proxy.setCount(count, ownerAddress);
	}

	public int getCount() {
		return proxy.getCount();
	}

	public Member getOwner() {
		return proxy.getOwner();
	}

	public CountDownLatchOperationsCounter getCountDownLatchOperationsCounter() {
		return proxy.getCountDownLatchOperationsCounter();
	}

	public String getLongName() {
		return proxy.getLongName();
	}

	public String getName() {
		return proxy.getName();
	}

	public void await() throws InstanceDestroyedException, MemberLeftException,
			InterruptedException {
		proxy.await();
	}

	public InstanceType getInstanceType() {
		return proxy.getInstanceType();
	}

	public void destroy() {
		checkPermission(new CountDownLatchPermission(getName(), SecurityConstants.ACTION_DESTROY));
		proxy.destroy();
	}

	public Object getId() {
		return proxy.getId();
	}

	public boolean await(long timeout, TimeUnit unit)
			throws InstanceDestroyedException, MemberLeftException,
			InterruptedException {
		return proxy.await(timeout, unit);
	}

	public void countDown() {
		checkCountDown();
		proxy.countDown();
	}

	public boolean hasCount() {
		return proxy.hasCount();
	}

	public boolean setCount(int count) {
		checkSet();
		return proxy.setCount(count);
	}

	public LocalCountDownLatchStats getLocalCountDownLatchStats() {
		checkPermission(new CountDownLatchPermission(getName(), SecurityConstants.ACTION_STATISTICS));
		return proxy.getLocalCountDownLatchStats();
	}

}