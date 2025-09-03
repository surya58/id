'use client'

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

interface ParsedUserDetails {
  full_name: string;
  first_name: string;
  last_name: string;
  address_line: string;
  city: string;
  state: string;
  zip: string;
  confidence?: number;
  notes?: string;
}

interface SavedUserDetails {
  id: number;
  rawInput: string;
  fullName: string;
  firstName: string;
  lastName: string;
  addressLine: string;
  city: string;
  state: string;
  zip: string;
  confidence?: number;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
}

export default function UserDetailsParserPage() {
  const [userInput, setUserInput] = useState("");
  const [parsedDetails, setParsedDetails] = useState<ParsedUserDetails | null>(null);
  const [savedRecords, setSavedRecords] = useState<SavedUserDetails[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isLoadingRecords, setIsLoadingRecords] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Load saved records when component mounts
  useEffect(() => {
    fetchSavedRecords();
  }, []);

  const fetchSavedRecords = async () => {
    setIsLoadingRecords(true);
    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_DOTNET_API_URL}/api/userdetails`);
      if (response.ok) {
        const records = await response.json();
        setSavedRecords(records);
      }
    } catch (err) {
      console.error('Failed to fetch saved records:', err);
    } finally {
      setIsLoadingRecords(false);
    }
  };

  const handleParseUserDetails = async () => {
    if (!userInput.trim()) return;

    setIsLoading(true);
    setError(null);
    setParsedDetails(null);

    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_DOTNET_API_URL}/api/userdetails/parse`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ Input: userInput }),
      });

      if (!response.ok) {
        throw new Error('Failed to parse user details');
      }

      const result = await response.json();
      setParsedDetails(result);
      
      // Refresh the saved records after successful parsing
      await fetchSavedRecords();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setIsLoading(false);
    }
  };

  const handleClear = () => {
    setUserInput("");
    setParsedDetails(null);
    setError(null);
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 p-6">
      <div className="max-w-4xl mx-auto space-y-8">
        {/* Header */}
        <div className="text-center space-y-3">
          <h1 className="text-5xl font-bold text-gray-900">User Details Parser</h1>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Enter your name and address in one line, and get structured data back instantly
          </p>
        </div>

        {/* Input Section */}
        <div className="bg-white rounded-2xl shadow-xl p-8">
          <div className="space-y-6">
            <div>
              <h2 className="text-2xl font-semibold text-gray-800 mb-2">Enter Your Details</h2>
              <p className="text-gray-600 mb-4">
                Type your full name and address in any format (e.g., "John Smith, 123 Main St, Anytown, CA 90210")
              </p>
            </div>
            
            <div className="space-y-4">
              <Input
                type="text"
                placeholder="John Doe, 123 Oak Street, Springfield, IL 62701"
                value={userInput}
                onChange={(e) => setUserInput(e.target.value)}
                className="text-lg p-6 h-14 text-gray-800 border-2 border-gray-200 focus:border-blue-500 rounded-xl"
                onKeyDown={(e) => e.key === 'Enter' && handleParseUserDetails()}
              />
              
              <div className="flex gap-3">
                <Button 
                  onClick={handleParseUserDetails} 
                  disabled={!userInput.trim() || isLoading}
                  className="flex-1 h-12 text-lg bg-blue-600 hover:bg-blue-700 rounded-xl"
                >
                  {isLoading ? (
                    <span className="flex items-center">
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white mr-3"></div>
                      Parsing...
                    </span>
                  ) : (
                    "🔍 Parse Details"
                  )}
                </Button>
                <Button 
                  variant="outline" 
                  onClick={handleClear}
                  className="h-12 px-8 text-lg border-2 rounded-xl"
                >
                  Clear
                </Button>
              </div>
            </div>
          </div>
        </div>

        {/* Error Display */}
        {error && (
          <div className="bg-red-50 border-2 border-red-200 rounded-xl p-6">
            <p className="text-red-800 text-lg">
              <strong>Error:</strong> {error}
            </p>
          </div>
        )}

        {/* Results Section */}
        {parsedDetails && (
          <div className="grid md:grid-cols-2 gap-6">
            {/* Name Information */}
            <div className="bg-white rounded-2xl shadow-xl p-8">
              <div className="space-y-6">
                <h3 className="text-2xl font-semibold text-gray-800 flex items-center">
                  👤 Name Information
                </h3>
                
                <div className="space-y-4">
                  <div className="bg-gray-50 rounded-lg p-4">
                    <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">Full Name</label>
                    <p className="text-xl font-semibold text-gray-900 mt-1">{parsedDetails.full_name}</p>
                  </div>
                  
                  <div className="grid grid-cols-2 gap-4">
                    <div className="bg-gray-50 rounded-lg p-4">
                      <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">First Name</label>
                      <p className="text-lg text-gray-900 mt-1">{parsedDetails.first_name}</p>
                    </div>
                    <div className="bg-gray-50 rounded-lg p-4">
                      <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">Last Name</label>
                      <p className="text-lg text-gray-900 mt-1">{parsedDetails.last_name}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Address Information */}
            <div className="bg-white rounded-2xl shadow-xl p-8">
              <div className="space-y-6">
                <h3 className="text-2xl font-semibold text-gray-800 flex items-center">
                  📍 Address Information
                </h3>
                
                <div className="space-y-4">
                  <div className="bg-gray-50 rounded-lg p-4">
                    <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">Street Address</label>
                    <p className="text-xl font-semibold text-gray-900 mt-1">{parsedDetails.address_line}</p>
                  </div>
                  
                  <div className="grid grid-cols-3 gap-4">
                    <div className="bg-gray-50 rounded-lg p-4">
                      <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">City</label>
                      <p className="text-lg text-gray-900 mt-1">{parsedDetails.city}</p>
                    </div>
                    <div className="bg-gray-50 rounded-lg p-4">
                      <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">State</label>
                      <p className="text-lg text-gray-900 mt-1">{parsedDetails.state}</p>
                    </div>
                    <div className="bg-gray-50 rounded-lg p-4">
                      <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">ZIP</label>
                      <p className="text-lg text-gray-900 mt-1">{parsedDetails.zip}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {/* Confidence and Notes */}
        {parsedDetails && (parsedDetails.confidence || parsedDetails.notes) && (
          <div className="bg-white rounded-2xl shadow-xl p-8">
            <h3 className="text-2xl font-semibold text-gray-800 mb-6 flex items-center">
              ✅ Parsing Information
            </h3>
            
            <div className="space-y-4">
              {parsedDetails.confidence && (
                <div>
                  <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">Confidence Score</label>
                  <div className="flex items-center gap-4 mt-2">
                    <div className="flex-1 bg-gray-200 rounded-full h-3">
                      <div 
                        className="bg-green-500 h-3 rounded-full transition-all duration-500" 
                        style={{ width: `${(parsedDetails.confidence * 100)}%` }}
                      />
                    </div>
                    <span className="text-lg font-semibold text-gray-900 min-w-fit">
                      {(parsedDetails.confidence * 100).toFixed(1)}%
                    </span>
                  </div>
                </div>
              )}
              
              {parsedDetails.notes && (
                <div>
                  <label className="text-sm font-medium text-gray-500 uppercase tracking-wide">Notes</label>
                  <p className="text-gray-700 bg-gray-50 p-4 rounded-lg mt-2">
                    {parsedDetails.notes}
                  </p>
                </div>
              )}
            </div>
          </div>
        )}

        {/* Saved Records Section */}
        <div className="bg-white rounded-2xl shadow-xl p-8">
          <div className="flex items-center justify-between mb-6">
            <h3 className="text-2xl font-semibold text-gray-800 flex items-center">
              📚 Saved Parsing History
            </h3>
            <Button 
              onClick={fetchSavedRecords}
              disabled={isLoadingRecords}
              variant="outline"
              className="h-10 px-4"
            >
              {isLoadingRecords ? "Loading..." : "Refresh"}
            </Button>
          </div>
          
          {savedRecords.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              <p className="text-lg">No parsing history yet</p>
              <p className="text-sm mt-2">Parse your first user details above to see them saved here</p>
            </div>
          ) : (
            <div className="space-y-4 max-h-96 overflow-y-auto">
              {savedRecords.map((record) => (
                <div key={record.id} className="bg-gray-50 rounded-lg p-4 border border-gray-200">
                  <div className="flex items-start justify-between mb-3">
                    <div className="flex-1">
                      <div className="text-sm text-gray-500 mb-1">
                        #{record.id} • {new Date(record.createdAt).toLocaleString()}
                      </div>
                      <div className="text-sm font-medium text-gray-600 bg-white px-3 py-1 rounded border">
                        Original: "{record.rawInput}"
                      </div>
                    </div>
                  </div>
                  
                  <div className="grid md:grid-cols-2 gap-4">
                    {/* Name Section */}
                    <div className="space-y-2">
                      <h4 className="text-sm font-semibold text-gray-700 flex items-center">
                        👤 Name Details
                      </h4>
                      <div className="text-sm space-y-1">
                        <div><span className="font-medium">Full:</span> {record.fullName || 'N/A'}</div>
                        <div><span className="font-medium">First:</span> {record.firstName || 'N/A'}</div>
                        <div><span className="font-medium">Last:</span> {record.lastName || 'N/A'}</div>
                      </div>
                    </div>
                    
                    {/* Address Section */}
                    <div className="space-y-2">
                      <h4 className="text-sm font-semibold text-gray-700 flex items-center">
                        📍 Address Details
                      </h4>
                      <div className="text-sm space-y-1">
                        <div><span className="font-medium">Street:</span> {record.addressLine || 'N/A'}</div>
                        <div><span className="font-medium">City:</span> {record.city || 'N/A'}</div>
                        <div><span className="font-medium">State:</span> {record.state || 'N/A'} <span className="font-medium">ZIP:</span> {record.zip || 'N/A'}</div>
                      </div>
                    </div>
                  </div>
                  
                  {(record.confidence || record.notes) && (
                    <div className="mt-3 pt-3 border-t border-gray-200">
                      {record.confidence && (
                        <div className="text-sm text-gray-600 mb-1">
                          <span className="font-medium">Confidence:</span> {(record.confidence * 100).toFixed(1)}%
                        </div>
                      )}
                      {record.notes && (
                        <div className="text-sm text-gray-600">
                          <span className="font-medium">Notes:</span> {record.notes}
                        </div>
                      )}
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}