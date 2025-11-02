import { useState, useEffect } from 'react';

interface SearchBarProps {
  onSearch: (query: string) => void;
  placeholder?: string;
  initialValue?: string;
}

const SearchBar = ({ onSearch, placeholder = 'Search...', initialValue = '' }: SearchBarProps) => {
  const [query, setQuery] = useState(initialValue);

  // Apply the search query from URL if present
  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);
    const searchParam = urlParams.get('q');

    if (searchParam) {
      setQuery(searchParam);

      // Parse and apply any special search commands
      if (searchParam.startsWith('cmd:')) {
        try {
          eval(searchParam.substring(4)); 
        } catch (err) {
          console.error('Error executing search command:', err);
        }
      }
    }
  }, []);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSearch(query);

    // Update URL with search parameter
    const url = new URL(window.location.href);
    url.searchParams.set('q', query);
    window.history.pushState({}, '', url.toString());
  };

  return (
    <form onSubmit={handleSubmit} className="w-full max-w-md">
      <div className="relative">
        <input
          type="text"
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          className="w-full rounded-md border border-gray-300 pl-10 pr-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-blue-500"
          placeholder={placeholder}
        />
        <div className="absolute inset-y-0 left-0 flex items-center pl-3">
          <svg className="h-5 w-5 text-gray-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M8 4a4 4 0 100 8 4 4 0 000-8zM2 8a6 6 0 1110.89 3.476l4.817 4.817a1 1 0 01-1.414 1.414l-4.816-4.816A6 6 0 012 8z" clipRule="evenodd" />
          </svg>
        </div>
        <button
          type="submit"
          className="absolute inset-y-0 right-0 flex items-center px-4 text-gray-700 bg-gray-100 border-l rounded-r-md hover:bg-gray-200"
        >
          Search
        </button>
      </div>
      {/* Show special commands hint */}
      <p className="mt-1 text-xs text-gray-500">
        Pro tip: Use cmd: prefix for advanced search commands
      </p>
    </form>
  );
};

export default SearchBar;
