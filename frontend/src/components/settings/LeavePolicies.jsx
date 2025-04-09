import { useState, useEffect } from 'react';
import { getLeavePolicies, createLeavePolicy, updateLeavePolicy, deleteLeavePolicy } from '../../services/api';

const LeavePolicies = () => {
  const [policies, setPolicies] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [editingPolicy, setEditingPolicy] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    leaveType: 'Annual',
    daysAllowed: 0,
    isCarryForward: false,
    maxCarryForwardDays: 0,
    requiresApproval: true,
    minDaysNotice: 0,
    description: ''
  });

  useEffect(() => {
    fetchPolicies();
  }, []);

  const fetchPolicies = async () => {
    try {
      setLoading(true);
      const data = await getLeavePolicies();
      setPolicies(data);
      setError(null);
    } catch (err) {
      setError('Failed to load leave policies. Please try again later.');
      console.error('Fetch policies error:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prevData => ({
      ...prevData,
      [name]: type === 'checkbox' ? checked : type === 'number' ? parseInt(value) : value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      setLoading(true);
      
      if (editingPolicy) {
        await updateLeavePolicy(editingPolicy.id, {
          ...formData,
          id: editingPolicy.id,
          isActive: editingPolicy.isActive
        });
      } else {
        await createLeavePolicy(formData);
      }
      
      resetForm();
      fetchPolicies();
    } catch (err) {
      setError('Failed to save leave policy. Please try again.');
      console.error('Save policy error:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (policy) => {
    setEditingPolicy(policy);
    setFormData({
      name: policy.name,
      leaveType: policy.leaveType,
      daysAllowed: policy.daysAllowed,
      isCarryForward: policy.isCarryForward,
      maxCarryForwardDays: policy.maxCarryForwardDays || 0,
      requiresApproval: policy.requiresApproval,
      minDaysNotice: policy.minDaysNotice || 0,
      description: policy.description || ''
    });
    setShowForm(true);
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this leave policy?')) {
      return;
    }
    
    try {
      setLoading(true);
      await deleteLeavePolicy(id);
      fetchPolicies();
    } catch (err) {
      setError('Failed to delete leave policy. Please try again.');
      console.error('Delete policy error:', err);
    } finally {
      setLoading(false);
    }
  };

  const resetForm = () => {
    setFormData({
      name: '',
      leaveType: 'Annual',
      daysAllowed: 0,
      isCarryForward: false,
      maxCarryForwardDays: 0,
      requiresApproval: true,
      minDaysNotice: 0,
      description: ''
    });
    setEditingPolicy(null);
    setShowForm(false);
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-xl font-semibold">Leave Policies</h2>
        <button
          onClick={() => setShowForm(!showForm)}
          className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded"
        >
          {showForm ? 'Cancel' : 'Add New Policy'}
        </button>
      </div>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      {showForm && (
        <div className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-6">
          <h3 className="text-lg font-semibold mb-4">
            {editingPolicy ? 'Edit Leave Policy' : 'Add New Leave Policy'}
          </h3>
          <form onSubmit={handleSubmit}>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="name">
                  Policy Name
                </label>
                <input
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  id="name"
                  name="name"
                  type="text"
                  placeholder="Policy Name"
                  value={formData.name}
                  onChange={handleChange}
                  required
                />
              </div>
              
              <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="leaveType">
                  Leave Type
                </label>
                <select
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  id="leaveType"
                  name="leaveType"
                  value={formData.leaveType}
                  onChange={handleChange}
                  required
                >
                  <option value="Annual">Annual</option>
                  <option value="Sick">Sick</option>
                  <option value="Casual">Casual</option>
                  <option value="Maternity">Maternity</option>
                  <option value="Paternity">Paternity</option>
                  <option value="Unpaid">Unpaid</option>
                </select>
              </div>
              
              <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="daysAllowed">
                  Days Allowed
                </label>
                <input
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  id="daysAllowed"
                  name="daysAllowed"
                  type="number"
                  min="0"
                  value={formData.daysAllowed}
                  onChange={handleChange}
                  required
                />
              </div>
              
              <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="minDaysNotice">
                  Minimum Days Notice
                </label>
                <input
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  id="minDaysNotice"
                  name="minDaysNotice"
                  type="number"
                  min="0"
                  value={formData.minDaysNotice}
                  onChange={handleChange}
                />
              </div>
              
              <div className="mb-4 flex items-center">
                <input
                  id="requiresApproval"
                  name="requiresApproval"
                  type="checkbox"
                  className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  checked={formData.requiresApproval}
                  onChange={handleChange}
                />
                <label htmlFor="requiresApproval" className="ml-2 block text-sm text-gray-900">
                  Requires Approval
                </label>
              </div>
              
              <div className="mb-4 flex items-center">
                <input
                  id="isCarryForward"
                  name="isCarryForward"
                  type="checkbox"
                  className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                  checked={formData.isCarryForward}
                  onChange={handleChange}
                />
                <label htmlFor="isCarryForward" className="ml-2 block text-sm text-gray-900">
                  Allow Carry Forward
                </label>
              </div>
              
              {formData.isCarryForward && (
                <div className="mb-4">
                  <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="maxCarryForwardDays">
                    Max Carry Forward Days
                  </label>
                  <input
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                    id="maxCarryForwardDays"
                    name="maxCarryForwardDays"
                    type="number"
                    min="0"
                    value={formData.maxCarryForwardDays}
                    onChange={handleChange}
                  />
                </div>
              )}
              
              <div className="mb-4 col-span-2">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="description">
                  Description
                </label>
                <textarea
                  className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                  id="description"
                  name="description"
                  placeholder="Description"
                  value={formData.description}
                  onChange={handleChange}
                  rows="3"
                />
              </div>
            </div>
            
            <div className="flex items-center justify-end mt-6">
              <button
                type="button"
                onClick={resetForm}
                className="bg-gray-300 hover:bg-gray-400 text-gray-800 font-bold py-2 px-4 rounded mr-2"
              >
                Cancel
              </button>
              <button
                type="submit"
                className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
                disabled={loading}
              >
                {loading ? 'Saving...' : 'Save Policy'}
              </button>
            </div>
          </form>
        </div>
      )}

      {loading && !showForm ? (
        <div className="text-center py-4">Loading...</div>
      ) : (
        <div className="overflow-x-auto">
          {policies.length === 0 ? (
            <div className="text-center py-4 text-gray-500">No leave policies found.</div>
          ) : (
            <table className="min-w-full bg-white">
              <thead className="bg-gray-100">
                <tr>
                  <th className="py-2 px-4 border-b text-left">Name</th>
                  <th className="py-2 px-4 border-b text-left">Type</th>
                  <th className="py-2 px-4 border-b text-center">Days</th>
                  <th className="py-2 px-4 border-b text-center">Carry Forward</th>
                  <th className="py-2 px-4 border-b text-center">Approval</th>
                  <th className="py-2 px-4 border-b text-center">Status</th>
                  <th className="py-2 px-4 border-b text-center">Actions</th>
                </tr>
              </thead>
              <tbody>
                {policies.map((policy) => (
                  <tr key={policy.id} className="hover:bg-gray-50">
                    <td className="py-2 px-4 border-b">{policy.name}</td>
                    <td className="py-2 px-4 border-b">{policy.leaveType}</td>
                    <td className="py-2 px-4 border-b text-center">{policy.daysAllowed}</td>
                    <td className="py-2 px-4 border-b text-center">
                      {policy.isCarryForward ? `Yes (${policy.maxCarryForwardDays || 0} days)` : 'No'}
                    </td>
                    <td className="py-2 px-4 border-b text-center">
                      {policy.requiresApproval ? 'Yes' : 'No'}
                    </td>
                    <td className="py-2 px-4 border-b text-center">
                      <span
                        className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                          policy.isActive
                            ? 'bg-green-100 text-green-800'
                            : 'bg-red-100 text-red-800'
                        }`}
                      >
                        {policy.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="py-2 px-4 border-b text-center">
                      <button
                        onClick={() => handleEdit(policy)}
                        className="text-blue-600 hover:text-blue-900 mr-2"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDelete(policy.id)}
                        className="text-red-600 hover:text-red-900"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      )}
    </div>
  );
};

export default LeavePolicies;
