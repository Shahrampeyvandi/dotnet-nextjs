/**
 * Utility functions for handling image URLs
 */

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5028/api';
const API_DOMAIN = API_BASE_URL.replace('/api', '');

/**
 * Gets the full URL for an image
 * @param imageUrl - The image URL (can be relative or absolute)
 * @returns The full URL to the image
 */
export function getImageUrl(imageUrl: string | null | undefined): string {
  if (!imageUrl) {
    return '/placeholder-image.png'; // Fallback placeholder
  }

  // If it's already a full URL (http/https), return as is
  if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
    return imageUrl;
  }

  // If it starts with /, it's a relative path from the API domain
  if (imageUrl.startsWith('/')) {
    return `${API_DOMAIN}${imageUrl}`;
  }

  // Otherwise, assume it's relative to the API domain
  return `${API_DOMAIN}/${imageUrl}`;
}

/**
 * Gets a placeholder image URL
 */
export function getPlaceholderImage(): string {
  return '/placeholder-image.png';
}

