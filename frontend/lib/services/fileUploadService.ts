const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5028/api';

export const fileUploadService = {
  uploadProductImage: async (file: File): Promise<string> => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${API_BASE_URL}/FileUpload/product-image`, {
      method: 'POST',
      credentials: 'include',
      body: formData,
    });

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to upload image');
    }

    const data = await response.json();
    return data.imageUrl;
  },

  deleteProductImage: async (imageUrl: string): Promise<void> => {
    const response = await fetch(
      `${API_BASE_URL}/FileUpload/product-image?imageUrl=${encodeURIComponent(imageUrl)}`,
      {
        method: 'DELETE',
        credentials: 'include',
      }
    );

    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to delete image');
    }
  },
};

