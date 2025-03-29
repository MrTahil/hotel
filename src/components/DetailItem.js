const DetailItem = ({ label, value, icon, highlight = false, small = false }) => {
    return (
      <div className={`flex ${small ? 'text-sm' : ''}`}>
        {icon && (
          <span className="material-symbols-outlined mr-3 text-gray-500">
            {icon}
          </span>
        )}
        <div className="flex-1">
          <p className="text-gray-500">{label}</p>
          <p className={`font-medium ${highlight ? 'text-green-600' : 'text-gray-900'}`}>
            {value}
          </p>
        </div>
      </div>
    );
  };
  
  export default DetailItem;