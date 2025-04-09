import { useState } from 'react';
import LeavePolicies from '../components/settings/LeavePolicies';
import Holidays from '../components/settings/Holidays';
import ApiDebugger from '../components/debug/ApiDebugger';
import { FaBug } from 'react-icons/fa';

const Settings = () => {
  const [activeTab, setActiveTab] = useState('leavePolicies');
  const [showDebugger, setShowDebugger] = useState(false);

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Settings</h1>
        <button
          onClick={() => setShowDebugger(!showDebugger)}
          className="bg-gray-600 text-white px-4 py-2 rounded-lg flex items-center"
        >
          <FaBug className="mr-2" /> {showDebugger ? 'Hide Debugger' : 'Show Debugger'}
        </button>
      </div>

      {showDebugger && <ApiDebugger />}

      <div className="mb-6">
        <div className="border-b border-gray-200">
          <nav className="-mb-px flex space-x-8">
            <button
              onClick={() => setActiveTab('leavePolicies')}
              className={`${
                activeTab === 'leavePolicies'
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
            >
              Leave Policies
            </button>
            <button
              onClick={() => setActiveTab('holidays')}
              className={`${
                activeTab === 'holidays'
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
            >
              Holidays
            </button>
          </nav>
        </div>
      </div>

      <div className="mt-6">
        {activeTab === 'leavePolicies' && <LeavePolicies />}
        {activeTab === 'holidays' && <Holidays />}
      </div>
    </div>
  );
};

export default Settings;
