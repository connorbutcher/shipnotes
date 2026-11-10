export interface Application {
  id: string;
  name: string;
  iconUrl: string | null;
  apiKeyPrefix: string;
  createdAt: string;
  updatedAt: string;
}

/** Returned when an API key is issued. The key is only ever available in this response. */
export interface ApplicationWithApiKey {
  application: Application;
  apiKey: string;
}

export interface SaveApplicationRequest {
  name: string;
  iconUrl: string | null;
}
