import React from 'react';

const BankDetailForm = ({ bankDetail, setBankDetail, errors = {} }) => {
  const handleChange = (e) => {
    const { name, value } = e.target;

    // Create a copy of the current bank detail state
    const updatedBankDetail = { ...bankDetail };

    // Update the specific field
    updatedBankDetail[name] = value;

    // Update the state with the new values
    setBankDetail(updatedBankDetail);

    console.log('Updated bank detail:', updatedBankDetail);
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 className="text-xl font-semibold mb-4">Bank Details</h2>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Bank Name*
          </label>
          <input
            type="text"
            name="bankName"
            value={bankDetail?.bankName || ''}
            onChange={handleChange}
            className={`w-full p-2 border rounded-md ${errors.bankName ? 'border-red-500' : 'border-gray-300'}`}
            placeholder="Enter bank name"
          />
          {errors.bankName && <p className="text-red-500 text-xs mt-1">{errors.bankName}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Account Holder Name*
          </label>
          <input
            type="text"
            name="accountHolderName"
            value={bankDetail?.accountHolderName || ''}
            onChange={handleChange}
            className={`w-full p-2 border rounded-md ${errors.accountHolderName ? 'border-red-500' : 'border-gray-300'}`}
            placeholder="Enter account holder name"
          />
          {errors.accountHolderName && <p className="text-red-500 text-xs mt-1">{errors.accountHolderName}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Account Number*
          </label>
          <input
            type="text"
            name="accountNumber"
            value={bankDetail?.accountNumber || ''}
            onChange={handleChange}
            className={`w-full p-2 border rounded-md ${errors.accountNumber ? 'border-red-500' : 'border-gray-300'}`}
            placeholder="Enter account number"
          />
          {errors.accountNumber && <p className="text-red-500 text-xs mt-1">{errors.accountNumber}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            IFSC Code*
          </label>
          <input
            type="text"
            name="ifscCode"
            value={bankDetail?.ifscCode || ''}
            onChange={handleChange}
            className={`w-full p-2 border rounded-md ${errors.ifscCode ? 'border-red-500' : 'border-gray-300'}`}
            placeholder="Enter IFSC code"
          />
          {errors.ifscCode && <p className="text-red-500 text-xs mt-1">{errors.ifscCode}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Branch Name
          </label>
          <input
            type="text"
            name="branchName"
            value={bankDetail?.branchName || ''}
            onChange={handleChange}
            className="w-full p-2 border border-gray-300 rounded-md"
            placeholder="Enter branch name"
          />
        </div>
      </div>
    </div>
  );
};

export default BankDetailForm;
